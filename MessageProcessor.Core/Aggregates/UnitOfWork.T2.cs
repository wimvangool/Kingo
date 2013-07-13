using System;
using System.Linq;

namespace YellowFlare.MessageProcessing.Aggregates
{
    /// <summary>
    /// Represents a Unit-Of-Work that can be flushed/comitted to a backing store. This class is a so-called
    /// read- and write-through implementation, and also implements a basic Identity Map.
    /// </summary>
    /// <typeparam name="TKey">Type of the key that identifies an aggregate.</typeparam>
    /// <typeparam name="TValue">Type of aggregates that are managed.</typeparam>
    public sealed class UnitOfWork<TKey, TValue> : IUnitOfWork
        where TKey : struct, IEquatable<TKey>
        where TValue : class, IAggregate<TKey>
    {        
        private readonly IAggregateStore<TKey, TValue> _store;
        private readonly string _flushGroup;
        private readonly bool _canBeFlushedAsynchronously;

        private readonly AggregateSet<TKey, TValue> _selectedAggregates;
        private readonly AggregateSet<TKey, TValue> _insertedAggregates;
        private readonly AggregateSet<TKey, TValue> _deletedAggregates;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork{TKey, TValue}" /> class.
        /// </summary>        
        /// <param name="store">Reference to the backing store where all aggregates can be read from and written to.</param>
        /// <param name="flushGroup">Indicates which group this unit of work belongs to.</param>
        /// <param name="canBeFlushedAsynchronously">
        /// Indicates whether or not the controller may flush this unit of work on a thread different than it was
        /// created on.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="store"/> is <c>null</c>.
        /// </exception>
        public UnitOfWork(IAggregateStore<TKey, TValue> store, string flushGroup = null, bool canBeFlushedAsynchronously = false)
        {            
            if (store == null)
            {
                throw new ArgumentNullException("store");
            }           
            _store = store;
            _flushGroup = flushGroup;
            _canBeFlushedAsynchronously = canBeFlushedAsynchronously;

            _selectedAggregates = new AggregateSet<TKey, TValue>();
            _insertedAggregates = new AggregateSet<TKey, TValue>();
            _deletedAggregates = new AggregateSet<TKey, TValue>();
        }

        /// <inheritdoc />
        public string FlushGroup
        {
            get { return _flushGroup; }
        }

        /// <inheritdoc />
        public bool CanBeFlushedAsynchronously
        {
            get { return _canBeFlushedAsynchronously; }
        }

        private bool HasAggregatesToDelete
        {
            get { return _deletedAggregates.Count > 0; }
        }

        private bool HasAggregatesToUpdate
        {
            get { return _selectedAggregates.Any(aggregate => aggregate.HasBeenUpdated); }
        }

        private bool HasAggregateToInsert
        {
            get { return _insertedAggregates.Count > 0; }
        }

        /// <summary>
        /// Tries to retrieve and return an aggregate that has the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key of the aggregate to find.</param>
        /// <param name="value">When found, this parameter will referenced the retrieved aggregate; otherwise <c>null</c>.</param>
        /// <returns>
        /// <c>true</c> if the aggregate with the specified <paramref name="key"/> was found; otherwise <c>false</c>.
        /// </returns>
        public bool TryFind(TKey key, out TValue value)
        {
            // In order to return the desired aggregate, we first try to find it in the 'cache': the aggregates
            // that have already been loaded or added in the current session. If so, we return that instance.
            if (_selectedAggregates.TryGetValue(key, out value) || _insertedAggregates.TryGetValue(key, out value))
            {
                return true;
            }
            // If the desired aggregate was not selected or inserted, and it was not deleted (in which case it
            // cannot be returned by definition), an attempt is made to retrieve it from the data store. If found,
            // we add it to the selected-aggregate set and enlist this UnitOfWork with the controller because it
            // might need to be flushed later.
            if (!_deletedAggregates.Contains(key) && _store.TrySelect(key, out value))
            {
                _selectedAggregates.Add(value);

                UnitOfWorkContext.Enlist(this);
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Marks the specified aggregate as 'inserted' or 'updated', depending on context.
        /// </summary>
        /// <param name="value">The aggregate to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public void Add(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            // When adding a new aggregate, we first check if it is not already present in cache. If so,
            // we just return.
            if (_selectedAggregates.Contains(value.Key) || _insertedAggregates.Contains(value.Key))
            {
                return;
            }
            // If, by any chance, the aggregate that is now being added was deleted previously, it is
            // restored again by moving it to the selected-aggregate set. In any other case, we add the
            // aggregate to the inserted-aggregates set and enlist to the controller to mark a fresh change.
            if (_deletedAggregates.Contains(value.Key))
            {
                _deletedAggregates.MoveAggregateTo(value.Key, _selectedAggregates);
            }
            else
            {
                _insertedAggregates.Add(value);

                UnitOfWorkContext.Enlist(this);
            }
        }

        /// <summary>
        /// Marks the specified aggregate as 'deleted'.
        /// </summary>
        /// <param name="value">The aggregate to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public void Remove(TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            // When removing an aggregate, we first check whether it has been loaded (selected) before, and if so,
            // simply move it to the deleted-aggregates set. If it was inserted before (a very strange but possible
            // situation), the previous insert is simply undone again. In any other case, the aggregate is first
            // loaded into memory and then marked deleted.
            if (_selectedAggregates.Contains(value.Key))
            {
                _selectedAggregates.MoveAggregateTo(value.Key, _deletedAggregates);
            }
            else if (_insertedAggregates.Contains(value.Key))
            {
                _insertedAggregates.Remove(value.Key);
            }
            else
            {
                _deletedAggregates.Add(value);

                UnitOfWorkContext.Enlist(this);                
            }
        }        

        /// <inheritdoc />
        public bool RequiresFlush()
        {
            return HasAggregatesToDelete || HasAggregatesToUpdate || HasAggregateToInsert;
        }

        /// <inheritdoc />
        public void Flush()
        {
            DeleteAggregates();
            UpdateAggregates();
            InsertAggregates();
        }

        private void DeleteAggregates()
        {
            foreach (var aggregate in _deletedAggregates)
            {
                _store.Delete(aggregate.Value);
            }
            _deletedAggregates.Clear();
        }

        private void UpdateAggregates()
        {
            foreach (var aggregate in _selectedAggregates.Where(aggregate => aggregate.HasBeenUpdated))
            {
                _store.Update(aggregate.Value, aggregate.OriginalVersion);
            }
            _selectedAggregates.Clear();
        }

        private void InsertAggregates()
        {
            foreach (var aggregate in _insertedAggregates)
            {
                _store.Insert(aggregate.Value);
            }
            _insertedAggregates.Clear();
        }        
    }
}
