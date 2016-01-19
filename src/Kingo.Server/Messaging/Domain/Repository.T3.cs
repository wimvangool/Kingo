using System;
using System.Text;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a repository of a certain type of aggregate. Besides the basic
    /// read- and write-operations, this call also implements an Identity Map so that
    /// implementers can focus on implementing serialization and deserialization of the
    /// aggregates to and from a backing store.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class Repository<TKey, TVersion, TAggregate> : IUnitOfWork, IDisposable                
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {                        
        private readonly AggregateSet<TKey, TVersion, TAggregate> _selectedAggregates;
        private readonly AggregateSet<TKey, TVersion, TAggregate> _insertedAggregates;        
        private readonly AggregateSet<TKey, TVersion, TAggregate> _deletedAggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="Kingo.Messaging.Domain.Repository{TKey,TVersion,TAggregate}" /> class.
        /// </summary>                
        internal Repository()
        {            
            _selectedAggregates = new AggregateSet<TKey, TVersion, TAggregate>();
            _insertedAggregates = new AggregateSet<TKey, TVersion, TAggregate>();
            _deletedAggregates = new AggregateSet<TKey, TVersion, TAggregate>();
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
        protected void Enlist()
        {
            var context = UnitOfWorkContext.Current;
            if (context != null)
            {
                context.Enlist(this);
            }
        }        

        private bool HasAggregatesToDelete
        {
            get { return _deletedAggregates.Count > 0; }
        }

        private bool HasAggregatesToUpdate
        {
            get { return _selectedAggregates.HasUpdates(); }
        }

        private bool HasAggregateToInsert
        {
            get { return _insertedAggregates.Count > 0; }
        }               

        /// <inheritdoc />
        public bool RequiresFlush()
        {
            return HasAggregatesToDelete || HasAggregatesToUpdate || HasAggregateToInsert;
        }              

        /// <summary>
        /// Flushes any pending changes to the underlying infrastructure.
        /// </summary>
        public async Task FlushAsync()
        {
            await FlushAsync(new DomainEventStream<TKey, TVersion>(UnitOfWorkContext.Current));          
        }        

        /// <summary>
        /// Flushes all pending changes of the aggregates while at the same time writing all pending events to
        /// the specified <paramref name="domainEventStream"/>.
        /// </summary>
        /// <param name="domainEventStream">The event stream to write the aggregate's events to.</param>
        /// <returns>A task representing the flush action.</returns>
        protected virtual async Task FlushAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            await DeleteAggregatesAsync(domainEventStream);
            await UpdateAggregatesAsync(domainEventStream);
            await InsertAggregatesAsync(domainEventStream);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var updateCount = _selectedAggregates.CountUpdatedAggregates();

            var info = new StringBuilder();
            info.Append('[');
            info.AppendFormat("Unchanged = {0}, ", _selectedAggregates.Count - updateCount);
            info.AppendFormat("Inserted = {0}, ", _insertedAggregates.Count);
            info.AppendFormat("Updated = {0}, ", updateCount);
            info.AppendFormat("Deleted = {0}", _deletedAggregates.Count);
            info.Append(']');
            return info.ToString();
        }

        #endregion

        #region [====== Getting and Updating ======]

        private sealed class SelectedAggregate : Aggregate<TKey, TVersion, TAggregate>
        {
            private readonly Repository<TKey, TVersion, TAggregate> _repository;
            private readonly TAggregate _aggregate;
            private TVersion _originalVersion;

            internal SelectedAggregate(Repository<TKey, TVersion, TAggregate> repository, TAggregate aggregate)
            {
                _repository = repository;
                _aggregate = aggregate;
                _originalVersion = aggregate.Version;
            }

            internal override TAggregate Value
            {
                get { return _aggregate; }
            }

            internal override bool Matches(TAggregate aggregate)
            {
                return ReferenceEquals(_aggregate, aggregate);
            }

            internal override bool HasBeenUpdated()
            {
                return _repository.HasBeenUpdated(_aggregate, _originalVersion);
            }            

            internal override async Task CommitAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
            {
                await CommitIfUpdatedAsync(domainEventStream);

                // When an updated aggregate is committed, it's new version becomes the original version.
                _originalVersion = _aggregate.Version;
            }

            private async Task CommitIfUpdatedAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
            {
                if (HasBeenUpdated())
                {
                    var updateSucceeded = await _repository.UpdateAsync(_aggregate, _originalVersion, domainEventStream);
                    if (updateSucceeded)
                    {
                        return;
                    }
                    throw NewConcurrencyException(_aggregate, _originalVersion);
                }                
            }

            private static Exception NewConcurrencyException(object aggregate, object originalVersion)
            {
                var messageFormat = ExceptionMessages.Repository_ConcurrenyException;
                var message = string.Format(messageFormat, aggregate.GetType(), originalVersion);
                return new ConcurrencyException(aggregate, originalVersion, message);
            }
        }

        /// <summary>
        /// Retrieves an <see cref="IHasVersion{T, S}" /> by its key.
        /// </summary>
        /// <param name="key">The key of the aggregate to return.</param>
        /// <returns>The aggregate with the specified <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="AggregateNotFoundByKeyException{T}">
        /// No aggregate of type <typeparamref name="TAggregate"/> with the specified <paramref name="key"/> was found.
        /// </exception>
        public async Task<TAggregate> GetByKeyAsync(TKey key)
        {
            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException("key");
            }
            TAggregate aggregate;

            // In order to return the desired aggregate, we first try to find it in the 'cache': the aggregates
            // that have already been loaded or added in the current session. If so, we return that instance.
            if (_selectedAggregates.TryGetValue(key, out aggregate) || _insertedAggregates.TryGetValue(key, out aggregate))
            {
                OnAggregateSelected(aggregate);

                return aggregate;
            }
            // If the desired aggregate was not selected or inserted, and it was not deleted (in which case it
            // cannot be returned by definition), an attempt is made to retrieve it from the data store. If found,
            // we add it to the selected-aggregate set and enlist this UnitOfWork with the controller because it
            // might need to be flushed later.
            if (_deletedAggregates.ContainsKey(key) || (aggregate = await SelectByKeyAndRestoreAsync(key)) == null || _deletedAggregates.ContainsValue(aggregate))
            {
                throw NewAggregateNotFoundByKeyException(key);
            }
            _selectedAggregates.Add(key, new SelectedAggregate(this, aggregate));

            OnAggregateSelected(aggregate);

            return aggregate; 
        }        

        internal abstract Task<TAggregate> SelectByKeyAndRestoreAsync(TKey key);               

        /// <summary>
        /// This method is called just before an aggregate is returned to clients, either from cache or from the repository.
        /// If you override this method, make sure you call the base implementation.
        /// </summary>
        /// <param name="aggregate">The aggregate that was requested.</param>
        protected virtual void OnAggregateSelected(TAggregate aggregate)
        {
            if (EnlistAutomatically)
            {
                Enlist();
            }
        }               

        private Task UpdateAggregatesAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            return _selectedAggregates.CommitAsync(domainEventStream);
        }

        internal abstract Task<bool> UpdateAsync(TAggregate aggregate, TVersion originalVersion, IWritableEventStream<TKey, TVersion> domainEventStream);        

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

        /// <summary>
        /// Creates and returns a new <see cref="AggregateNotFoundByKeyException{T}" /> that indicates that this repository
        /// was unable to retrieve an aggregate of type <typeparamref name="TAggregate"/> with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of the aggregate that was not found.</param>
        /// <returns>A new <see cref="AggregateNotFoundByKeyException{T}" />.</returns>
        protected AggregateNotFoundByKeyException<T> NewAggregateNotFoundByKeyException<T>(T key)            
        {
            var messageFormat = ExceptionMessages.Repository_AggregateNotFoundByKey;
            var message = string.Format(messageFormat, typeof(TAggregate), key);
            return new AggregateNotFoundByKeyException<T>(typeof(TAggregate), key, message);
        }

        #endregion

        #region [====== Adding and Inserting ======]

        private sealed class AddedAggregate : Aggregate<TKey, TVersion, TAggregate>
        {
            private readonly Repository<TKey, TVersion, TAggregate> _repository;
            private readonly TAggregate _aggregate;

            internal AddedAggregate(Repository<TKey, TVersion, TAggregate> repository, TAggregate aggregate)
            {
                _repository = repository;
                _aggregate = aggregate;
            }

            internal override TAggregate Value
            {
                get { return _aggregate; }
            }

            internal override bool Matches(TAggregate aggregate)
            {
                return ReferenceEquals(_aggregate, aggregate);
            }

            internal override bool HasBeenUpdated()
            {
                return false;
            }            

            internal override async Task CommitAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
            {                
                await _repository.InsertAsync(_aggregate, domainEventStream);

                // When a newly inserted aggregate is committed, it is transferred to the selected set.
                _repository._insertedAggregates.RemoveByKey(_aggregate.Key);
                _repository._selectedAggregates.Add(_aggregate.Key, new SelectedAggregate(_repository, _aggregate));
            }
        }

        /// <summary>
        /// Marks the specified aggregate as 'inserted' or 'updated', depending on context.
        /// </summary>
        /// <param name="aggregate">The aggregate to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        public void Add(TAggregate aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            // When adding a new aggregate, we first check if it is not already present in cache. If so,
            // we can ignore this operation. However, if a different aggregate is added with a key that
            // is already assigned to another aggregate, an exception is thrown.
            if (_selectedAggregates.ContainsValue(aggregate) || _insertedAggregates.ContainsValue(aggregate))
            {
                return;
            }
            if (_selectedAggregates.ContainsKey(aggregate.Key) || _insertedAggregates.ContainsKey(aggregate.Key))
            {
                throw NewDuplicateKeyException(aggregate.Key);
            }
            _insertedAggregates.Add(aggregate.Key, new AddedAggregate(this, aggregate));

            OnAggregateAdded(aggregate);         
        }

        /// <summary>
        /// This method is called just after a new aggregate was added to this repository. If you override
        /// this method, make sure you call the base implementation.
        /// </summary>
        /// <param name="aggregate">The aggregate that was in.</param>
        protected virtual void OnAggregateAdded(TAggregate aggregate)
        {
            if (EnlistAutomatically)
            {
                Enlist();
            }
        }

        private Task InsertAggregatesAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            return _insertedAggregates.CommitAsync(domainEventStream);            
        }        
                        
        internal abstract Task InsertAsync(TAggregate aggregate, IWritableEventStream<TKey, TVersion> domainEventStream);                

        /// <summary>
        /// Creates and returns a new <see cref="DuplicateKeyException{T}" /> indicating that an aggregate could not
        /// be added to the repository because an aggregate with the same key already exists.
        /// </summary>
        /// <typeparam name="T">Type of the key.</typeparam>
        /// <param name="key">The key that was already present.</param>
        /// <returns>A new <see cref="DuplicateKeyException{T}" /> that can be thrown when an insert failed.</returns>
        protected static Exception NewDuplicateKeyException<T>(T key)           
        {
            var messageFormat = ExceptionMessages.Repository_DuplicateKey;
            var message = string.Format(messageFormat, key);
            return new DuplicateKeyException<T>(key, message);
        }

        #endregion

        #region [====== Removing and Deleting ======]

        private sealed class RemovedAggregate : Aggregate<TKey, TVersion, TAggregate>           
        {
            private readonly Repository<TKey, TVersion, TAggregate> _repository;
            private readonly TKey _key;            

            internal RemovedAggregate(Repository<TKey, TVersion, TAggregate> repository, TKey key)
            {
                _repository = repository;
                _key = key;                
            }

            internal override TAggregate Value
            {
                get { return null; }
            }

            internal override bool Matches(TAggregate aggregate)
            {
                return aggregate.Key.Equals(_key);
            }

            internal override bool HasBeenUpdated()
            {
                return false;
            }            

            internal override async Task CommitAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
            {                
                await _repository.DeleteAsync(_key, domainEventStream);

                _repository._deletedAggregates.RemoveByKey(_key);
            }
        }

        /// <summary>
        /// Marks the aggregate with the specified <paramref name="key"/> as deleted.
        /// </summary>
        /// <param name="key">Key of the aggregate to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is <c>null</c>.
        /// </exception>
        public void RemoveByKey(TKey key)
        {
            if (ReferenceEquals(key, null))
            {
                throw new ArgumentNullException("key");
            }
            // When removing an aggregate, we first check whether it has been loaded (selected) before, and if so,
            // simply move it to the deleted-aggregates set. If it was inserted before (a very strange but possible
            // situation), the previous insert is simply undone again. In any other case, the aggregate is first
            // loaded into memory and then marked deleted.
            TAggregate aggregate;

            if (_selectedAggregates.TryGetValue(key, out aggregate))
            {
                _selectedAggregates.RemoveByKey(key);
                _deletedAggregates.Add(key, new RemovedAggregate(this, key));

                OnAggregateRemoved(key);
            }
            else if (_insertedAggregates.ContainsKey(key))
            {
                _insertedAggregates.RemoveByKey(key);
            }
            else
            {
                _deletedAggregates.Add(key, new RemovedAggregate(this, key));

                OnAggregateRemoved(key);
            }
        }        

        /// <summary>
        /// This method is called just after a new aggregate was removed from this repository. If you override
        /// this method, make sure you call the base implementation.
        /// </summary>
        /// <param name="key">Key of the aggregate that was removed.</param>
        protected virtual void OnAggregateRemoved<T>(T key)            
        {
            if (EnlistAutomatically)
            {
                Enlist();
            }
        }

        private Task DeleteAggregatesAsync(IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            return _deletedAggregates.CommitAsync(domainEventStream);
        }

        /// <summary>
        /// Deletes / removes a certain instance from the underlying Data Store.
        /// </summary>
        /// <param name="key">Key of the aggregate to remove.</param> 
        /// <param name="domainEventStream">
        /// Stream that can be used to publish any events that result from an aggregate being deleted.
        /// </param>       
        protected virtual Task DeleteAsync(TKey key, IWritableEventStream<TKey, TVersion> domainEventStream)
        {
            throw NewDeleteNotSupportedException();
        }

        private static Exception NewDeleteNotSupportedException()
        {
            var messageFormat = ExceptionMessages.Repository_DeleteNotSupported;
            var message = string.Format(messageFormat, typeof(TAggregate).Name);
            return new NotSupportedException(message);
        }

        #endregion                
    }
}
