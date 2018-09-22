using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Resources;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging.Domain
{
    internal sealed class UnitOfWork<TKey, TAggregate> : IRepository<TKey, TAggregate>, IUnitOfWork
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        #region [====== AggregateState ======]

        private abstract class AggregateState
        {           
            protected AggregateState(UnitOfWork<TKey, TAggregate> unitOfWork, bool isChangeState)
            {
                UnitOfWork = unitOfWork;
                IsChangeState = isChangeState;
            }

            protected Repository<TKey, TAggregate> Repository =>
                UnitOfWork._repository;            
             
            protected UnitOfWork<TKey, TAggregate> UnitOfWork
            {
                get;
            }

            public abstract TKey AggregateId
            {
                get;
            }

            public bool IsChangeState
            {
                get;
            }

            protected virtual void Enter() { }

            protected virtual void Exit() { }

            public virtual void AddToChangeSet(ChangeSet<TKey> changeSet) { }

            public AggregateState Commit(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory) =>
                MoveToState(this, CreateCommittedState(unitOfWork, keepAggregatesInMemory));

            protected abstract AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory);

            protected void MoveToState(AggregateState newState) =>
                UnitOfWork.OnMovedToNewState(MoveToState(this, newState));
            
            private static AggregateState MoveToState(AggregateState oldState, AggregateState newState)
            {
                oldState.Exit();
                newState.Enter();
                return newState;
            }                                      

            public abstract Task<TAggregate> GetByIdAsync();

            public abstract Task<bool> AddAsync(TAggregate aggregate);

            public abstract Task<bool> RemoveByIdAsync();          

            protected static Exception NewDuplicateKeyException(TKey id)
            {
                var messageFormat = ExceptionMessages.Repository_DuplicateKeyException_AggregateAlreadyExists;
                var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), id);
                return new DuplicateKeyException(id, message);
            }
        }

        private sealed class NullState : AggregateState
        {            
            private readonly TKey _id;

            public NullState(UnitOfWork<TKey, TAggregate> unitOfWork, TKey id) :
                base(unitOfWork, false)
            {                
                _id = id;
            }                      

            public override TKey AggregateId =>
                _id;          

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory) =>
                new NullState(unitOfWork, _id);

            public override async Task<TAggregate> GetByIdAsync()
            {                
                var aggregate = await Repository.SelectByIdAndRestoreAsync(AggregateId);
                if (aggregate == null)
                {
                    throw NewAggregateNotFoundException(AggregateId);
                }
                MoveToState(new UnmodifiedState(UnitOfWork, aggregate));
                return aggregate;
            }

            public override async Task<bool> AddAsync(TAggregate aggregate)
            {
                var aggregateData = await Repository.SelectByIdAsync(aggregate.Id);
                if (aggregateData != null)
                {
                    throw NewDuplicateKeyException(aggregate.Id);
                }
                MoveToState(new AddedState(UnitOfWork, aggregate));
                return true;
            }            

            public override async Task<bool> RemoveByIdAsync()
            {                
                var aggregate = await Repository.SelectByIdAndRestoreAsync(AggregateId);
                if (aggregate == null)
                {
                    return false;
                }
                MoveToState(new RemovedState(UnitOfWork, aggregate, false));
                return true;
            }           

            private static Exception NewAggregateNotFoundException(TKey id)
            {
                var messageFormat = ExceptionMessages.Repository_AggregateNotFound;
                var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), id);
                return new AggregateNotFoundException(id, message);
            }            
        }

        private abstract class NonNullState : AggregateState
        {
            protected NonNullState(UnitOfWork<TKey, TAggregate> unitOfWork, bool isChangeState, TAggregate aggregate) :
                base(unitOfWork, isChangeState)
            {
                Aggregate = aggregate;
            }

            public override TKey AggregateId =>
                Aggregate.Id;

            protected TAggregate Aggregate
            {
                get;
            }

            protected override void Enter() =>
                Aggregate.EventPublished += HandleEventPublished;

            protected override void Exit() =>
                Aggregate.EventPublished -= HandleEventPublished;

            protected virtual void HandleEventPublished(object sender, EventPublishedEventArgs e) =>
                Publish(e.Event);

            protected void Publish(IEvent @event) =>
                Repository.Context.EventBus.Publish(@event);
        }

        private sealed class UnmodifiedState : NonNullState
        {                        
            public UnmodifiedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork, false, aggregate) { }

            protected override void HandleEventPublished(object sender, EventPublishedEventArgs e)
            {
                base.HandleEventPublished(sender, e);

                MoveToState(new ModifiedState(UnitOfWork, Aggregate));
            }

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory)
            {
                if (keepAggregatesInMemory)
                {
                    return new UnmodifiedState(unitOfWork, Aggregate);
                }
                return new NullState(unitOfWork, AggregateId);
            }               

            public override Task<TAggregate> GetByIdAsync() =>
                Task.FromResult(Aggregate);

            public override Task<bool> AddAsync(TAggregate aggregate) => Run(() =>
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return false;
                }
                throw NewDuplicateKeyException(aggregate.Id);
            });                            

            public override Task<bool> RemoveByIdAsync() => Run(() =>
            {
                MoveToState(new RemovedState(UnitOfWork, Aggregate, false));
                return true;
            });
        }

        private sealed class AddedState : NonNullState
        {                        
            public AddedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork, true, aggregate) { }

            protected override void Enter()
            {
                base.Enter();

                foreach (var @event in Aggregate.Events)
                {
                    Publish(@event);
                }
            }

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) =>
                changeSet.AddAggregateToInsert(Aggregate);

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory)
            {
                if (keepAggregatesInMemory)
                {
                    return new UnmodifiedState(unitOfWork, Aggregate);
                }
                return new NullState(unitOfWork, AggregateId);
            }                

            public override Task<TAggregate> GetByIdAsync() =>
                Task.FromResult(Aggregate);

            public override Task<bool> AddAsync(TAggregate aggregate) => Run(() =>
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return false;
                }
                throw NewDuplicateKeyException(AggregateId);
            });

            public override async Task<bool> RemoveByIdAsync()
            {
                MoveToState(new RemovedState(UnitOfWork, Aggregate, true));
                return true;
            }                
        }

        private sealed class ModifiedState : NonNullState
        {                        
            public ModifiedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate) :
                base(unitOfWork, true, aggregate) { }                                                            

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) =>
                changeSet.AddAggregateToUpdate(Aggregate);

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory)
            {
                if (keepAggregatesInMemory)
                {
                    return new UnmodifiedState(unitOfWork, Aggregate);
                }
                return new NullState(unitOfWork, AggregateId);
            }                

            public override Task<TAggregate> GetByIdAsync() =>
                Task.FromResult(Aggregate);

            public override async Task<bool> AddAsync(TAggregate aggregate)
            {
                if (ReferenceEquals(Aggregate, aggregate))
                {
                    return false;
                }
                throw NewDuplicateKeyException(AggregateId);
            }

            public override async Task<bool> RemoveByIdAsync()
            {
                MoveToState(new RemovedState(UnitOfWork, Aggregate, false));
                return true;
            }
        }

        private sealed class RemovedState : NonNullState
        {                        
            private readonly bool _hasBeenAddedInSession;

            public RemovedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate, bool hasBeenAddedInSession) :
                base(unitOfWork, true, aggregate)
            {                                
                _hasBeenAddedInSession = hasBeenAddedInSession;
            }                                  

            protected override void Enter()
            {
                base.Enter();

                Aggregate.NotifyRemoved();
            }                

            public override void AddToChangeSet(ChangeSet<TKey> changeSet)
            {                
                if (Aggregate.Events.Count > 0)
                {
                    if (_hasBeenAddedInSession)
                    {
                        changeSet.AddAggregateToInsert(Aggregate);
                    }
                    else
                    {
                        changeSet.AddAggregateToUpdate(Aggregate);
                    }
                }                
                changeSet.AddAggregateToDelete(Aggregate.Id);
            }                

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork, bool keepAggregatesInMemory) =>
                new NullState(unitOfWork, Aggregate.Id);

            public override Task<TAggregate> GetByIdAsync() =>
                throw NewAggregateRemovedException(AggregateId);

            public override Task<bool> AddAsync(TAggregate aggregate) =>
                throw NewDuplicateKeyException(AggregateId);

            public override Task<bool> RemoveByIdAsync() =>
                Task.FromResult(false);

            private static Exception NewAggregateRemovedException(TKey aggregateId)
            {
                var messageFormat = ExceptionMessages.Repository_AggregateRemovedInSession;
                var message = string.Format(messageFormat, typeof(TAggregate).FriendlyName(), aggregateId);
                return new AggregateNotFoundException(aggregateId, message);
            }
        }        

        #endregion

        private readonly Repository<TKey, TAggregate> _repository;
        private readonly AggregateSerializationStrategy _serializationStrategy;
        private readonly Dictionary<TKey, AggregateState> _aggregates;
        private bool _requiresFlush;

        public UnitOfWork(Repository<TKey, TAggregate> repository, AggregateSerializationStrategy serializationStrategy)
        {
            if (IsNotValid(serializationStrategy))
            {
                throw NewInvalidSerializationStrategyException(serializationStrategy);
            }
            _repository = repository;
            _serializationStrategy = serializationStrategy;
            _aggregates = new Dictionary<TKey, AggregateState>();
        }        

        public AggregateSerializationStrategy SerializationStrategy =>
            _serializationStrategy;        

        private static bool IsNotValid(AggregateSerializationStrategy serializationStrategy) =>
            !serializationStrategy.UsesSnapshots() && !serializationStrategy.UsesEvents();       

        private static Exception NewInvalidSerializationStrategyException(AggregateSerializationStrategy serializationStrategy)
        {
            var messageFormat = ExceptionMessages.UnitOfWork_InvalidSerializationStrategySpecified;
            var message = string.Format(messageFormat, serializationStrategy);
            return new ArgumentOutOfRangeException(nameof(serializationStrategy), message);
        }

        public UnitOfWork<TKey, TAggregate> Commit(bool keepAggregatesInMemory)
        {
            var unitOfWork = new UnitOfWork<TKey, TAggregate>(_repository, _serializationStrategy);
            var committedChanges = unitOfWork._aggregates;

            foreach (var aggregate in _aggregates)
            {
                committedChanges.Add(aggregate.Key, aggregate.Value.Commit(unitOfWork, keepAggregatesInMemory));
            }
            return unitOfWork;
        }

        /// <inheritdoc />
        public override string ToString() =>
            BuildChangeSet().ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        public Task<TAggregate> GetByIdAsync(TKey id) =>
            GetAggregateState(id).GetByIdAsync();

        public Task<bool> AddAsync(TAggregate aggregate) =>
            GetAggregateState(aggregate).AddAsync(aggregate);

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
            if (_aggregates.TryGetValue(id, out var state))
            {
                return state;
            }   
            return new NullState(this, id);         
        }        

        private void OnMovedToNewState(AggregateState newState)
        {            
            if ((_aggregates[newState.AggregateId] = newState).IsChangeState)
            {
                _requiresFlush = true;
                _repository.Context.UnitOfWork.EnlistAsync(_repository, _repository.ResourceId).Await(_repository.Context.Token);
            }
        }        

        #endregion

        #region [====== IUnitOfWork ======]

        public bool RequiresFlush() =>
            _requiresFlush;

        public Task FlushAsync() =>
            _repository.FlushAsync(BuildChangeSet());

        private ChangeSet<TKey> BuildChangeSet()
        {
            var changeSet = new ChangeSet<TKey>(SerializationStrategy);

            foreach (var aggregate in _aggregates.Values)
            {
                aggregate.AddToChangeSet(changeSet);
            }
            return changeSet;
        }

        #endregion
    }
}
