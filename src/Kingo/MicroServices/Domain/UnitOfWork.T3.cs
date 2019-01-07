using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.MicroServices.Domain
{
    internal sealed class UnitOfWork<TKey, TVersion, TAggregate> : IRepository<TKey, TAggregate>
        where TKey : struct, IEquatable<TKey>
        where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        where TAggregate : class, IAggregateRoot<TKey, TVersion>
    {
        #region [====== AggregateState ======]

        private abstract class AggregateState
        {           
            protected AggregateState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork)
            {
                UnitOfWork = unitOfWork;                
            }

            protected Repository<TKey, TVersion, TAggregate> Repository =>
                UnitOfWork._repository;            
             
            protected UnitOfWork<TKey, TVersion, TAggregate> UnitOfWork
            {
                get;
            }

            public abstract TKey AggregateId
            {
                get;
            }            

            public virtual void Enter() { }

            public virtual void Exit() { }            

            protected void MoveToState(AggregateState newState) =>
                UnitOfWork.MoveToNewState(newState);                                                           

            public abstract Task<TAggregate> GetByIdOrNullAsync();

            public abstract Task<bool> AddAsync(TAggregate aggregate);

            public abstract Task<bool> RemoveAsync(TAggregate aggregate);

            public abstract Task<bool> RemoveByIdAsync();

            public abstract AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory);

            protected static Exception NewDuplicateKeyException(TKey id)
            {
                var messageFormat = ExceptionMessages.Repository_DuplicateKeyException_AggregateAlreadyExists;
                var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), id);
                return new DuplicateKeyException(id, message);
            }
        }

        #endregion

        #region [====== NullState ======]

        private sealed class NullState : AggregateState
        {            
            private readonly TKey _id;

            public NullState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TKey id) :
                base(unitOfWork)
            {                
                _id = id;
            }            

            public override TKey AggregateId =>
                _id;                                             

            public override async Task<TAggregate> GetByIdOrNullAsync()
            {                
                var dataSet = await Repository.SelectByIdAsync(AggregateId);
                if (dataSet == null)
                {
                    return null;
                }
                var aggregate = RestoreAggregate(dataSet);
                if (aggregate.HasBeenRemoved)
                {
                    return null;
                }
                MoveToState(new UnmodifiedState(UnitOfWork, aggregate, dataSet.Events.Count));
                return aggregate;
            }

            public override async Task<bool> AddAsync(TAggregate aggregate)
            {
                var dataSet = await Repository.SelectByIdAsync(aggregate.Id);
                if (dataSet != null)
                {
                    throw NewDuplicateKeyException(aggregate.Id);
                }
                MoveToState(new AddedState(UnitOfWork, aggregate));
                return true;
            }

            public override Task<bool> RemoveAsync(TAggregate aggregate) =>
                Task.FromResult(false);

            public override async Task<bool> RemoveByIdAsync()
            {                
                var dataSet = await Repository.SelectByIdAsync(AggregateId);
                if (dataSet == null)
                {
                    return false;
                }
                MoveToState(new RemovedAfterUpdateState(UnitOfWork, RestoreAggregate(dataSet), default, dataSet.Events.Count));
                return true;
            }

            private TAggregate RestoreAggregate(AggregateDataSet dataSet) =>                
                UnitOfWork._serializationStrategy.Deserialize<TKey, TVersion, TAggregate>(dataSet, MessageHandlerContext.Current?.EventBus);

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory) =>
                new NullState(UnitOfWork, _id);
        }

        #endregion

        #region [====== NonNullState ======]

        private abstract class NonNullState : AggregateState
        {
            protected NonNullState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork)
            {
                Aggregate = aggregate;
            }

            public override TKey AggregateId =>
                Aggregate.Id;

            protected TAggregate Aggregate
            {
                get;
            }

            public override void Enter() =>
                Aggregate.Modified += HandleAggregateModified;

            public override void Exit() =>
                Aggregate.Modified -= HandleAggregateModified;

            protected virtual void HandleAggregateModified(object sender, EventArgs e) { }

            public override async Task<bool> RemoveAsync(TAggregate aggregate)
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return await RemoveByIdAsync();
                }
                return false;
            }
        }

        #endregion

        #region [====== UnmodifiedState ======]

        private sealed class UnmodifiedState : NonNullState
        {
            private readonly TVersion _oldVersion;
            private readonly int _eventsSinceLastSnapshot;

            public UnmodifiedState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate, int eventsSinceLastSnapshot) :
                base(unitOfWork, aggregate)
            {
                _oldVersion = aggregate.Version;
                _eventsSinceLastSnapshot = eventsSinceLastSnapshot;
            }           

            protected override void HandleAggregateModified(object sender, EventArgs e)
            {
                base.HandleAggregateModified(sender, e);
                MoveToState(new ModifiedState(UnitOfWork, Aggregate, _oldVersion, _eventsSinceLastSnapshot));
            }                               

            public override Task<TAggregate> GetByIdOrNullAsync() =>
                Task.FromResult(Aggregate);

            public override Task<bool> AddAsync(TAggregate aggregate) => AsyncMethod.Run(() =>
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return false;
                }
                throw NewDuplicateKeyException(aggregate.Id);
            });            

            public override Task<bool> RemoveByIdAsync() => AsyncMethod.Run(() =>
            {                
                MoveToState(new RemovedAfterUpdateState(UnitOfWork, Aggregate, _oldVersion, _eventsSinceLastSnapshot));
                return true;
            });

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory)
            {
                if (keepAggregatesInMemory)
                {
                    return new UnmodifiedState(UnitOfWork, Aggregate, _eventsSinceLastSnapshot);
                }
                return new NullState(UnitOfWork, AggregateId);
            }
        }

        #endregion

        #region [====== AddedState ======]

        private sealed class AddedState : NonNullState
        {            
            public AddedState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork, aggregate) { }

            public override void Enter()
            {
                base.Enter();
                UnitOfWork._insertCount++;
            }

            public override void Exit()
            {
                UnitOfWork._insertCount--;
                base.Exit();
            }

            public override Task<TAggregate> GetByIdOrNullAsync() =>
                Task.FromResult(Aggregate);

            public override Task<bool> AddAsync(TAggregate aggregate) => AsyncMethod.Run(() =>
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return false;
                }
                throw NewDuplicateKeyException(AggregateId);
            });

            public override Task<bool> RemoveByIdAsync() => AsyncMethod.Run(() =>
            {
                MoveToState(new RemovedAfterInsertState(UnitOfWork, Aggregate));
                return true;
            });

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory)
            {
                var eventsSinceLastSnapshot = changeSet.AddAggregateToInsert(Aggregate);

                if (keepAggregatesInMemory)
                {
                    return new UnmodifiedState(UnitOfWork, Aggregate, eventsSinceLastSnapshot);
                }
                return new NullState(UnitOfWork, AggregateId);
            }
        }

        #endregion

        #region [====== ModifiedState ======]

        private sealed class ModifiedState : NonNullState
        {
            private readonly TVersion _oldVersion;
            private readonly int _eventsSinceLastSnapshot;

            public ModifiedState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate, TVersion oldVersion, int eventsSinceLastSnapshot) :
                base(unitOfWork, aggregate)
            {
                _oldVersion = oldVersion;
                _eventsSinceLastSnapshot = eventsSinceLastSnapshot;
            }

            public override void Enter()
            {
                base.Enter();
                UnitOfWork._updateCount++;
                UnitOfWork.EnlistRepository();                
            }           

            public override void Exit()
            {
                UnitOfWork._updateCount--;
                base.Exit();
            }

            public override Task<TAggregate> GetByIdOrNullAsync() =>
                Task.FromResult(Aggregate);

            public override Task<bool> AddAsync(TAggregate aggregate) => AsyncMethod.Run(() =>
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return false;
                }
                throw NewDuplicateKeyException(AggregateId);
            });

            public override Task<bool> RemoveByIdAsync() => AsyncMethod.Run(() =>
            {
                MoveToState(new RemovedAfterUpdateState(UnitOfWork, Aggregate, _oldVersion, _eventsSinceLastSnapshot));
                return true;
            });

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory)
            {
                var eventsSinceLastSnapshot = changeSet.AddAggregateToUpdate(Aggregate, _oldVersion, _eventsSinceLastSnapshot);

                if (keepAggregatesInMemory)
                {
                    return new UnmodifiedState(UnitOfWork, Aggregate, eventsSinceLastSnapshot);
                }
                return new NullState(UnitOfWork, AggregateId);
            }
        }

        #endregion

        #region [====== RemovedState ======]

        private abstract class RemovedState : NonNullState
        {            
            protected RemovedState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork, aggregate) { }

            protected bool SoftDeleteAggregate =>
                !HardDeleteAggregate;
            
            private bool HardDeleteAggregate
            {
                get;
                set;
            }

            public override void Enter()
            {
                base.Enter();            
                
                if (HardDeleteAggregate = Aggregate.NotifyRemoved())
                {
                    UnitOfWork._deleteCount++;
                }                
            }

            public override void Exit()
            {
                if (HardDeleteAggregate)
                {
                    UnitOfWork._deleteCount--;
                }
                base.Exit();
            }

            public override Task<TAggregate> GetByIdOrNullAsync() =>
                Task.FromResult<TAggregate>(null);

            public override Task<bool> AddAsync(TAggregate aggregate) =>
                throw NewDuplicateKeyException(AggregateId);

            public override Task<bool> RemoveByIdAsync() =>
                Task.FromResult(false);

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory)
            {
                if (HardDeleteAggregate)
                {
                    changeSet.AddAggregateToDelete(Aggregate.Id);
                }                
                return new NullState(UnitOfWork, Aggregate.Id);
            }
        }        

        private sealed class RemovedAfterInsertState : RemovedState
        {
            public RemovedAfterInsertState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork, aggregate) { }

            public override void Enter()
            {
                base.Enter();

                if (SoftDeleteAggregate)
                {
                    UnitOfWork._insertCount++;
                }
            }

            public override void Exit()
            {
                if (SoftDeleteAggregate)
                {
                    UnitOfWork._insertCount--;
                }
                base.Exit();
            }

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory)
            {
                if (SoftDeleteAggregate)
                {
                    changeSet.AddAggregateToInsert(Aggregate);
                }
                return base.Commit(changeSet, keepAggregatesInMemory);
            }
        }

        private sealed class RemovedAfterUpdateState : RemovedState
        {            
            private readonly TVersion _oldVersion;
            private readonly int _eventsSinceLastSnapshot;            

            public RemovedAfterUpdateState(UnitOfWork<TKey, TVersion, TAggregate> unitOfWork, TAggregate aggregate, TVersion oldVersion, int eventsSinceLastSnapshot) :
                base(unitOfWork, aggregate)
            {                
                _oldVersion = oldVersion;
                _eventsSinceLastSnapshot = eventsSinceLastSnapshot;
            }

            public override void Enter()
            {
                base.Enter();

                if (SoftDeleteAggregate)
                {
                    UnitOfWork._updateCount++;
                }
            }

            public override void Exit()
            {
                if (SoftDeleteAggregate)
                {
                    UnitOfWork._updateCount--;
                }
                base.Exit();
            }

            public override AggregateState Commit(ChangeSet<TKey, TVersion> changeSet, bool keepAggregatesInMemory)
            {
                if (SoftDeleteAggregate)
                {
                    changeSet.AddAggregateToUpdate(Aggregate, _oldVersion, _eventsSinceLastSnapshot);
                }                
                return base.Commit(changeSet, keepAggregatesInMemory);
            }
        }

        #endregion

        private readonly Repository<TKey, TVersion, TAggregate> _repository;
        private readonly SerializationStrategy _serializationStrategy;
        private readonly Dictionary<TKey, AggregateState> _aggregateStates;
        private int _insertCount;
        private int _updateCount;
        private int _deleteCount;        

        public UnitOfWork(Repository<TKey, TVersion, TAggregate> repository, SerializationStrategy serializationStrategy)
        {            
            _repository = repository;
            _serializationStrategy = serializationStrategy ?? throw new ArgumentNullException(nameof(serializationStrategy));
            _aggregateStates = new Dictionary<TKey, AggregateState>();
        }

        private void EnlistRepository() =>
            EnlistRepository(TimeSpan.FromMinutes(1), MessageHandlerContext.Current?.Token);

        private void EnlistRepository(TimeSpan timeout, CancellationToken? token)
        {
            if (_repository.OnAggregateModifiedAsync().Await(timeout, token))
            {
                return;
            }
            throw NewEnlistTimeoutException(timeout);
        }

        private static Exception NewEnlistTimeoutException(TimeSpan timeout)
        {
            var messageFormat = ExceptionMessages.UnitOfWork_EnlistmentTimeout;
            var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), timeout);
            return new TimeoutException(message);
        }

        public bool RequiresFlush =>
            _insertCount + _updateCount + _deleteCount > 0;

        public async Task FlushAsync(bool keepAggregatesInMemory)
        {
            var changeSet = Commit(keepAggregatesInMemory);

            try
            {
                await _repository.FlushAsync(changeSet);
            }
            catch
            {
                _aggregateStates.Clear();
                throw;
            }            
        }   
        
        private IChangeSet<TKey, TVersion> Commit(bool keepAggregatesInMemory)
        {
            var statesToCommit = _aggregateStates.Values.ToArray();
            var changeSet = new ChangeSet<TKey, TVersion>(_serializationStrategy);

            foreach (var state in statesToCommit)
            {
                MoveToNewState(state.Commit(changeSet, keepAggregatesInMemory));
            }
            return changeSet;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"[Inserted: {_insertCount}, Updated: {_updateCount}, Deleted: {_deleteCount}]";

        #region [====== IRepository<TKey, TAggregate> ======]

        public async Task<TAggregate> GetByIdAsync(TKey id)
        {
            var aggregate = await GetByIdOrNullAsync(id);
            if (aggregate == null)
            {
                throw NewAggregateNotFoundException(id);
            }
            return aggregate;
        }

        public Task<TAggregate> GetByIdOrNullAsync(TKey id) =>
            GetAggregateState(id).GetByIdOrNullAsync();

        public Task<bool> AddAsync(TAggregate aggregate) =>
            GetAggregateState(aggregate).AddAsync(aggregate);

        public async Task<bool> RemoveAsync(TAggregate aggregate)
        {
            if (aggregate == null)
            {
                return false;
            }
            return await GetAggregateState(aggregate.Id).RemoveAsync(aggregate);
        }

        public Task<bool> RemoveByIdAsync(TKey id) =>
            GetAggregateState(id).RemoveByIdAsync();

        private AggregateState GetAggregateState(TAggregate aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }
            return GetAggregateState(aggregate.Id);
        }

        private AggregateState GetAggregateState(TKey id)
        {
            if (_aggregateStates.TryGetValue(id, out var state))
            {
                return state;
            }   
            return new NullState(this, id);         
        }        

        private void MoveToNewState(AggregateState newState)
        {         
            if (_aggregateStates.TryGetValue(newState.AggregateId, out var oldState))
            {
                oldState.Exit();
            }
            (_aggregateStates[newState.AggregateId] = newState).Enter();
        }

        private static Exception NewAggregateNotFoundException(TKey id)
        {
            var messageFormat = ExceptionMessages.Repository_AggregateNotFound;
            var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), id);
            return new AggregateNotFoundException(id, message);
        }

        #endregion        
    }
}
