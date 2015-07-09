using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Syztem.Resources;

namespace Syztem.ComponentModel.Server.Domain
{
    /// <summary>
    /// Represents a UnitOfWork that can be flushed and comitted to a backing store. This class is a so-called
    /// read- and write-through implementation, and also implements a basic Identity Map.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class Repository<TAggregate, TKey, TVersion> : IUnitOfWork, IDisposable
        where TAggregate : class, IVersionedObject<TKey, TVersion>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>        
    {
        private readonly SemaphoreSlim _lock;
        private readonly AggregateRootSet<TAggregate, TKey, TVersion> _selectedAggregates;
        private readonly AggregateRootSet<TAggregate, TKey, TVersion> _insertedAggregates;
        private readonly AggregateRootSet<TAggregate, TKey, TVersion> _deletedAggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TKey, TVersion, TAggregate}" /> class.
        /// </summary>                
        protected Repository()
        {
            _lock = new SemaphoreSlim(1);
            _selectedAggregates = new AggregateRootSet<TAggregate, TKey, TVersion>();
            _insertedAggregates = new AggregateRootSet<TAggregate, TKey, TVersion>();
            _deletedAggregates = new AggregateRootSet<TAggregate, TKey, TVersion>();
        }

        /// <summary>
        /// Returns the lock that is used to synchronize reads and writes.
        /// </summary>
        protected SemaphoreSlim Lock
        {
            get { return _lock; }
        }

        #region [====== Dispose ======]

        /// <summary>
        /// Indicates whether not this instance has been disposed.
        /// </summary>
        protected bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Indicates if the method was called by the application explicitly (<c>true</c>), or by the finalizer
        /// (<c>false</c>).
        /// </param>
        /// <remarks>
        /// If <paramref name="disposing"/> is <c>true</c>, this method will dispose any managed resources immediately.
        /// Otherwise, only unmanaged resources will be released.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
            if (disposing)
            {
                Lock.Dispose();
            }
            IsDisposed = true;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ObjectDisposedException" />.
        /// </summary>
        /// <returns>A new <see cref="ObjectDisposedException" />.</returns>
        protected ObjectDisposedException NewObjectDisposedException()
        {
            return new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region [====== IUnitOfWork ======]

        /// <summary>
        /// Indicates whether or not this repository must enlist with the <see cref="MessageProcessor" />
        /// automatically when (possible) changes are detected. Default is <c>true</c>.
        /// </summary>
        protected virtual bool EnlistAutomatically
        {
            get { return true; }
        }

        /// <summary>
        /// Enlists this repository with the <see cref="MessageProcessor" /> to schedule it for a flush.
        /// </summary>
        protected virtual void Enlist()
        {
            MessageProcessor.Enlist(this);
        }        

        private bool HasAggregatesToDelete
        {
            get { return _deletedAggregates.Count > 0; }
        }

        private bool HasAggregatesToUpdate
        {
            get { return _selectedAggregates.Trackers.Any(tracker => HasBeenUpdated(tracker.Aggregate, tracker.OriginalVersion)); }
        }

        private bool HasAggregateToInsert
        {
            get { return _insertedAggregates.Count > 0; }
        }               

        /// <inheritdoc />
        public virtual bool RequiresFlush()
        {
            return HasAggregatesToDelete || HasAggregatesToUpdate || HasAggregateToInsert;
        }              

        /// <summary>
        /// Flushes any pending changes to the underlying infrastructure.
        /// </summary>
        public virtual async Task FlushAsync()
        {            
            await Lock.WaitAsync();

            try
            {
                await DeleteAggregatesAsync();
                await UpdateAggregatesAsync();
                await InsertAggregatesAsync();
            }
            finally
            {
                Lock.Release();
            }            
        }        

        #endregion

        #region [====== Getting and Updating ======]

        /// <summary>
        /// Retrieves an <see cref="IVersionedObject{T, S}" /> by its key.
        /// </summary>
        /// <param name="key">The key of the aggregate to return.</param>
        /// <returns>The aggregate with the specified <paramref name="key"/>.</returns>
        /// <exception cref="AggregateNotFoundByKeyException{T}">
        /// No aggregate of type <typeparamref name="TAggregate"/> with the specified <paramref name="key"/> was found.
        /// </exception>
        public virtual async Task<TAggregate> GetByKeyAsync(TKey key)
        {            
            await Lock.WaitAsync();

            try
            {
                TAggregate aggregate;

                // In order to return the desired aggregate, we first try to find it in the 'cache': the aggregates
                // that have already been loaded or added in the current session. If so, we return that instance.
                if (_selectedAggregates.TryGetValue(key, out aggregate) || _insertedAggregates.TryGetValue(key, out aggregate))
                {
                    return aggregate;
                }
                // If the desired aggregate was not selected or inserted, and it was not deleted (in which case it
                // cannot be returned by definition), an attempt is made to retrieve it from the data store. If found,
                // we add it to the selected-aggregate set and enlist this UnitOfWork with the controller because it
                // might need to be flushed later.
                if (!_deletedAggregates.Contains(key) && (aggregate = await SelectByKeyAsync(key)) != null)
                {
                    _selectedAggregates.Add(aggregate);

                    if (EnlistAutomatically)
                    {
                        Enlist();
                    }
                    return aggregate;
                }
                throw NewAggregateNotFoundByKeyException(key);                
            }
            finally
            {
                Lock.Release();
            }            
        }                

        /// <summary>
        /// Attempts to retrieve a specific Aggregate instance by its key. Returns <c>true</c>
        /// if found; returns <c>false</c> otherwise.
        /// </summary>
        /// <param name="key">Key of the Aggregate instance to retrieve.</param>        
        /// <returns>
        /// <c>True</c> if this store contains an instance with the specified <paramref name="key"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        protected abstract Task<TAggregate> SelectByKeyAsync(TKey key);

        private async Task UpdateAggregatesAsync()
        {
            var updates = from tracker in _selectedAggregates.Trackers
                          where HasBeenUpdated(tracker.Aggregate, tracker.OriginalVersion)
                          select UpdateAsync(tracker.Aggregate, tracker.OriginalVersion);

            await Task.WhenAll(updates);

            lock (_selectedAggregates)
            {
                _selectedAggregates.CommitUpdates();
            }
        }        

        /// <summary>
        /// Updates / overwrites a previous version with a new version.
        /// </summary>
        /// <param name="aggregate">The instance to update.</param>
        /// <param name="originalVersion">The version to overwrite.</param>
        /// <exception cref="ConstraintViolationException">
        /// The specified <paramref name="aggregate"/> could not be updated because it would violate a
        /// data-constraint in the data store.
        /// </exception>
        protected abstract Task UpdateAsync(TAggregate aggregate, TVersion originalVersion);

        /// <summary>
        /// Determines whether or not the specified <paramref name="aggregate"/> has been updated.
        /// </summary>
        /// <param name="aggregate">The aggregate to check.</param>
        /// <param name="originalVersion">The version of the aggregate when it was first retrieved.</param>
        /// <returns>
        /// <c>true</c> if this aggregate was updated; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// The default implementation will simply compare the <paramref name="originalVersion"/>
        /// with the current version of the aggregate to determine if it was updated, assuming that
        /// with every update the version was incremented. If you use another versioning-approach
        /// when updating aggregates, you need to override this method and implement your own strategy.
        /// </remarks>
        protected virtual bool HasBeenUpdated(TAggregate aggregate, TVersion originalVersion)
        {
            return aggregate.Version.CompareTo(originalVersion) > 0;
        }

        #endregion

        #region [====== Adding and Inserting ======]

        /// <summary>
        /// Marks the specified aggregate as 'inserted' or 'updated', depending on context.
        /// </summary>
        /// <param name="aggregate">The aggregate to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        public virtual void Add(TAggregate aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            Lock.Wait();

            try
            {
                // When adding a new aggregate, we first check if it is not already present in cache. If so,
                // we can ignore this operation. However, if a different aggregate is added with a key that
                // is already assigned to another aggregate, an exception is thrown.
                if (_selectedAggregates.Contains(aggregate) || _insertedAggregates.Contains(aggregate))
                {
                    return;
                }
                else if (_selectedAggregates.Contains(aggregate.Key) || _insertedAggregates.Contains(aggregate.Key))
                {
                    throw NewDuplicateKeyException(aggregate.Key);
                }
                
                _insertedAggregates.Add(aggregate);

                if (EnlistAutomatically)
                {
                    Enlist();
                }
            }
            finally
            {
                Lock.Release();
            }            
        }

        private async Task InsertAggregatesAsync()
        {
            await Task.WhenAll(_insertedAggregates.Trackers.Select(tracker => InsertAsync(tracker.Aggregate)));

            lock (_selectedAggregates)
            {
                foreach (var insertedAggregate in _insertedAggregates.Trackers)
                {
                    _insertedAggregates.CopyAggregateTo(insertedAggregate.Aggregate.Key, _selectedAggregates);
                }
            }   
            _insertedAggregates.Clear();
        }        

        /// <summary>
        /// Inserts the specified instance into this DataStore.
        /// </summary>
        /// <param name="aggregate">The instance to insert.</param>  
        /// <exception cref="ConstraintViolationException">
        /// The specified <paramref name="aggregate"/> could not be inserted because it would violate a
        /// data-constraint in the data store.
        /// </exception> 
        protected abstract Task InsertAsync(TAggregate aggregate);

        #endregion

        #region [====== Removing and Deleting ======]

        /// <summary>
        /// Marks the aggregate with the specified <paramref name="key"/> as deleted.
        /// </summary>
        /// <param name="key">Key of the aggregate to remove.</param>
        public virtual void RemoveByKey(TKey key)
        {
            Lock.Wait();

            try
            {
                // When removing an aggregate, we first check whether it has been loaded (selected) before, and if so,
                // simply move it to the deleted-aggregates set. If it was inserted before (a very strange but possible
                // situation), the previous insert is simply undone again. In any other case, the aggregate is first
                // loaded into memory and then marked deleted.
                if (_selectedAggregates.Contains(key))
                {
                    _selectedAggregates.CopyAggregateTo(key, _deletedAggregates);
                    _selectedAggregates.Remove(key);
                }
                else if (_insertedAggregates.Contains(key))
                {
                    _insertedAggregates.Remove(key);
                }
                else
                {
                    _deletedAggregates.Add(key);

                    if (EnlistAutomatically)
                    {
                        Enlist();
                    }
                }
            }
            finally
            {
                Lock.Release();
            }  
        }

        private async Task DeleteAggregatesAsync()
        {
            await Task.WhenAll(_deletedAggregates.Keys.Select(DeleteAsync));
            
            _deletedAggregates.Clear();
        }        

        /// <summary>
        /// Deletes / removes a certain instance from this DataStore.
        /// </summary>
        /// <param name="key">Key of the aggregate to remove.</param>
        /// <exception cref="ConstraintViolationException">
        /// The aggregate with specified <paramref name="key"/> could not be deleted because it would violate a
        /// data-constraint in the data store.
        /// </exception>
        protected abstract Task DeleteAsync(TKey key);

        #endregion

        #region [====== Exception Factory Methods ======]

        /// <summary>
        /// Creates and returns a new <see cref="AggregateNotFoundByKeyException{T}" /> that indicates that this repository
        /// was unable to retrieve an aggregate of type <typeparamref name="TAggregate"/> with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of the aggregate that was not found.</param>
        /// <returns>A new <see cref="AggregateNotFoundByKeyException{T}" />.</returns>
        protected AggregateNotFoundByKeyException<TKey> NewAggregateNotFoundByKeyException(TKey key)
        {
            var messageFormat = ExceptionMessages.Repository_AggregateNotFoundByKey;
            var message = string.Format(messageFormat, typeof(TAggregate), key);
            return new AggregateNotFoundByKeyException<TKey>(typeof(TAggregate), key, message);
        }        

        private static Exception NewDuplicateKeyException(TKey key)
        {
            var messageFormat = ExceptionMessages.Repository_DuplicateKey;
            var message = string.Format(messageFormat, key);
            return new DuplicateKeyException<TAggregate, TKey>(key, message);
        }

        #endregion
    }
}
