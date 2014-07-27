using System.Linq;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a UnitOfWork that can be flushed and comitted to a backing store. This class is a so-called
    /// read- and write-through implementation, and also implements a basic Identity Map.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TVersion">Type of the version of the aggregate.</typeparam>
    /// <typeparam name="TAggregate">Type of aggregates that are managed.</typeparam>
    public abstract class Repository<TKey, TVersion, TAggregate> : IUnitOfWork
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>
        where TAggregate : class, IAggregate<TKey, TVersion>
    {                
        private readonly AggregateSet<TKey, TVersion, TAggregate> _selectedAggregates;
        private readonly AggregateSet<TKey, TVersion, TAggregate> _insertedAggregates;
        private readonly AggregateSet<TKey, TVersion, TAggregate> _deletedAggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TKey, TVersion, TAggregate}" /> class.
        /// </summary>                
        protected Repository()
        {                                   
            _selectedAggregates = new AggregateSet<TKey, TVersion, TAggregate>();
            _insertedAggregates = new AggregateSet<TKey, TVersion, TAggregate>();
            _deletedAggregates = new AggregateSet<TKey, TVersion, TAggregate>();
        }

        #region [====== IUnitOfWork Implementation ======]

        Guid IUnitOfWork.FlushGroupId
        {
            get { return FlushGroupId; }
        }

        /// <summary>
        /// Indicates which group this unit of work belongs to.
        /// </summary>
        /// <remarks>
        /// The default implementation returns the <see cref="Guid.Empty">empty Guid</see>, which
        /// prevents this <see cref="Repository{S, T, U}" /> from grouping with other
        /// <see cref="IUnitOfWork">units of work</see>.
        /// </remarks>
        protected virtual Guid FlushGroupId
        {
            get { return Guid.Empty; }
        }

        bool IUnitOfWork.CanBeFlushedAsynchronously
        {
            get { return CanBeFlushedAsynchronously; }
        }

        /// <summary>
        /// Indicates whether or not the controller may flush this unit of work on a thread different than it was
        /// created on.
        /// </summary>
        /// <remarks>
        /// The default implementation returns <c>false</c>.
        /// </remarks>
        protected virtual bool CanBeFlushedAsynchronously
        {
            get { return false; }
        }

        bool IUnitOfWork.RequiresFlush()
        {
            return RequiresFlush();
        }

        /// <summary>
        /// Indicates whether or not the unit of work maintains any changes that need to flushed.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current instance needs to be flushed; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool RequiresFlush()
        {
            return HasAggregatesToDelete || HasAggregatesToUpdate || HasAggregateToInsert;
        }

        private bool HasAggregatesToDelete
        {
            get { return _deletedAggregates.Count > 0; }
        }

        private bool HasAggregatesToUpdate
        {
            get { return _selectedAggregates.Any(tracker => HasBeenUpdated(tracker.Aggregate, tracker.OriginalVersion)); }
        }

        private bool HasAggregateToInsert
        {
            get { return _insertedAggregates.Count > 0; }
        }

        void IUnitOfWork.Flush()
        {
            Flush();
        }

        /// <summary>
        /// Flushes any pending changes to the underlying infrastructure.
        /// </summary>
        protected virtual void Flush()
        {
            DeleteAggregates();
            UpdateAggregates();
            InsertAggregates();
        }        

        #endregion

        #region [====== Getting and Updating ======]

        /// <summary>
        /// Retrieves an <see cref="IAggregate{T, S}" /> by its key.
        /// </summary>
        /// <param name="key">The key of the aggregate to return.</param>
        /// <returns>The aggregate with the specified <paramref name="key"/>.</returns>
        /// <exception cref="AggregateNotFoundException">
        /// No aggregate of type <typeparamref name="TAggregate"/> with the specified <paramref name="key"/> was found.
        /// </exception>
        public virtual TAggregate GetByKey(TKey key)
        {
            TAggregate aggregate;

            if (TryGetByKey(key, out aggregate))
            {
                return aggregate;
            }
            throw AggregateNotFoundException.NewAggregateNotFoundException(key, typeof(TAggregate));
        }

        /// <summary>
        /// Tries to retrieve and return an aggregate that has the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of the aggregate to find.</param>
        /// <param name="aggregate">When found, this parameter will referenced the retrieved aggregate; otherwise <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if the aggregate with the specified <paramref name="key"/> was found; otherwise <c>false</c>.
        /// </returns>
        public virtual bool TryGetByKey(TKey key, out TAggregate aggregate)
        {
            // In order to return the desired aggregate, we first try to find it in the 'cache': the aggregates
            // that have already been loaded or added in the current session. If so, we return that instance.
            if (_selectedAggregates.TryGetValue(key, out aggregate) || _insertedAggregates.TryGetValue(key, out aggregate))
            {
                return true;
            }
            // If the desired aggregate was not selected or inserted, and it was not deleted (in which case it
            // cannot be returned by definition), an attempt is made to retrieve it from the data store. If found,
            // we add it to the selected-aggregate set and enlist this UnitOfWork with the controller because it
            // might need to be flushed later.
            if (!_deletedAggregates.Contains(key) && TrySelect(key, out aggregate))
            {
                _selectedAggregates.Add(aggregate);

                UnitOfWorkContext.Enlist(this);
            }
            aggregate = null;
            return false;
        }

        /// <summary>
        /// Attempts to retrieve a specific Aggregate instance by its key. Returns <c>true</c>
        /// if found; returns <c>false</c> otherwise.
        /// </summary>
        /// <param name="key">Key of the Aggregate instance to retrieve.</param>
        /// <param name="aggregate">
        /// This argument will be set to the retrieved instance if retrieval was succesful. Will
        /// be set to <c>null</c> otherwise.
        /// </param>
        /// <returns>
        /// <c>True</c> if this store contains an instance with the specified <paramref name="key"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        protected abstract bool TrySelect(TKey key, out TAggregate aggregate);

        private void UpdateAggregates()
        {
            foreach (var aggregate in _selectedAggregates.Where(aggregate => HasBeenUpdated(aggregate.Aggregate, aggregate.OriginalVersion)))
            {
                Update(aggregate.Aggregate, aggregate.OriginalVersion);
            }
            _selectedAggregates.Clear();
        }

        /// <summary>
        /// Updates / overwrites a previous version with a new version.
        /// </summary>
        /// <param name="aggregate">The instance to update.</param>
        /// <param name="originalVersion">The version to overwrite.</param>
        protected abstract void Update(TAggregate aggregate, TVersion originalVersion);

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
            return !aggregate.Version.Equals(originalVersion);
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
            // When adding a new aggregate, we first check if it is not already present in cache. If so,
            // we can ignore this operation.
            if (_selectedAggregates.Contains(aggregate.Key) || _insertedAggregates.Contains(aggregate.Key))
            {
                return;
            }
            // If, by any chance, the aggregate that is now being added was deleted previously, it is
            // restored again by moving it to the selected-aggregate set. In any other case, we add the
            // aggregate to the inserted-aggregates set and enlist to the controller to mark a fresh change.
            if (_deletedAggregates.Contains(aggregate.Key))
            {
                _deletedAggregates.MoveAggregateTo(aggregate.Key, _selectedAggregates);
            }
            else
            {
                _insertedAggregates.Add(aggregate);

                UnitOfWorkContext.Enlist(this);
            }
        }

        private void InsertAggregates()
        {
            foreach (var aggregate in _insertedAggregates)
            {
                Insert(aggregate.Aggregate);
            }
            _insertedAggregates.Clear();
        }

        /// <summary>
        /// Inserts the specified instance into this DataStore.
        /// </summary>
        /// <param name="aggregate">The instance to insert.</param>   
        protected abstract void Insert(TAggregate aggregate);

        #endregion

        #region [====== Removing and Deleting ======]

        /// <summary>
        /// Marks the specified aggregate as 'deleted'.
        /// </summary>
        /// <param name="aggregate">The aggregate to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="aggregate"/> is <c>null</c>.
        /// </exception>
        public virtual void Remove(TAggregate aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            // When removing an aggregate, we first check whether it has been loaded (selected) before, and if so,
            // simply move it to the deleted-aggregates set. If it was inserted before (a very strange but possible
            // situation), the previous insert is simply undone again. In any other case, the aggregate is first
            // loaded into memory and then marked deleted.
            if (_selectedAggregates.Contains(aggregate.Key))
            {
                _selectedAggregates.MoveAggregateTo(aggregate.Key, _deletedAggregates);
            }
            else if (_insertedAggregates.Contains(aggregate.Key))
            {
                _insertedAggregates.Remove(aggregate.Key);
            }
            else
            {
                _deletedAggregates.Add(aggregate);

                UnitOfWorkContext.Enlist(this);                
            }
        }

        private void DeleteAggregates()
        {
            foreach (var aggregate in _deletedAggregates)
            {
                Delete(aggregate.Aggregate);
            }
            _deletedAggregates.Clear();
        }

        /// <summary>
        /// Deletes / removes a certain instance from this DataStore.
        /// </summary>
        /// <param name="aggregate">The instance to remove.</param>
        protected abstract void Delete(TAggregate aggregate);

        #endregion
    }
}
