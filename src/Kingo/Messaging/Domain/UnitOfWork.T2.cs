using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging.Domain
{
    internal sealed class UnitOfWork<TKey, TAggregate> : IRepository<TKey, TAggregate>, IUnitOfWork
        where TKey : struct, IEquatable<TKey>
        where TAggregate : class, IAggregateRoot<TKey>
    {
        #region [====== AggregateState ======]

        private abstract class AggregateState
        {           
            protected Repository<TKey, TAggregate> Repository =>
                UnitOfWork._repository;
             
            protected abstract UnitOfWork<TKey, TAggregate> UnitOfWork
            {
                get;
            }

            protected abstract TKey AggregateId
            {
                get;
            }

            protected void MoveToState(AggregateState newState)
            {
                Exit();
                Enter(UnitOfWork._aggregates[AggregateId] = newState);
            }

            private void Enter(AggregateState newState)
            {
                if (newState.Enter())
                {
                    UnitOfWork._requiresFlush = true;
                    Repository.Context.UnitOfWork.Enlist(Repository, Repository.ResourceId);
                }
            }

            protected abstract bool Enter();

            protected abstract void Exit();

            public abstract void AddToChangeSet(ChangeSet<TKey> changeSet);

            public AggregateState Commit(UnitOfWork<TKey, TAggregate> unitOfWork)
            {
                var oldState = this;
                var newState = CreateCommittedState(unitOfWork);
                    
                oldState.Exit();
                newState.Enter();

                return newState;
            }

            protected abstract AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork);

            public abstract Task<TAggregate> GetByIdAsync();

            public abstract Task AddAsync(TAggregate aggregate);

            public abstract Task<bool> RemoveByIdAsync();            
        }

        private sealed class NullState : AggregateState
        {
            private readonly UnitOfWork<TKey, TAggregate> _unitOfWork;
            private readonly TKey _id;

            public NullState(UnitOfWork<TKey, TAggregate> unitOfWork, TKey id)
            {
                _unitOfWork = unitOfWork;
                _id = id;
            }            

            protected override UnitOfWork<TKey, TAggregate> UnitOfWork =>
                _unitOfWork;

            protected override TKey AggregateId =>
                _id;

            protected override bool Enter() =>
                false;

            protected override void Exit() { }

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) { }

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork) =>
                new NullState(unitOfWork, _id);

            public override async Task<TAggregate> GetByIdAsync()
            {
                var aggregate = await Repository.SelectByIdAndRestoreAsync(AggregateId);
                MoveToState(new UnmodifiedState(UnitOfWork, aggregate));
                return aggregate;
            }

            public override Task AddAsync(TAggregate aggregate)
            {
                throw new NotImplementedException();
            }

            public override Task<bool> RemoveByIdAsync()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class UnmodifiedState : AggregateState
        {
            private readonly UnitOfWork<TKey, TAggregate> _unitOfWork;
            private readonly TAggregate _aggregate;

            public UnmodifiedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate)                
            {
                _unitOfWork = unitOfWork;
                _aggregate = aggregate;
            }

            protected override UnitOfWork<TKey, TAggregate> UnitOfWork =>
                _unitOfWork;

            protected override TKey AggregateId =>
                _aggregate.Id;

            protected override bool Enter()
            {
                _aggregate.EventPublished += HandleEventPublished;
                return false;
            }

            protected override void Exit() =>
                _aggregate.EventPublished -= HandleEventPublished;

            private void HandleEventPublished(object sender, EventPublishedEventArgs e) =>
                MoveToState(new ModifiedState(UnitOfWork, _aggregate, e.WriteEventTo(Repository.Context.OutputStream)));

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) { }

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork) =>
                new UnmodifiedState(unitOfWork, _aggregate);

            public override Task<TAggregate> GetByIdAsync() =>
                AsyncMethod.Value(_aggregate);

            public override Task AddAsync(TAggregate aggregate)
            {
                throw new NotImplementedException();
            }

            public override Task<bool> RemoveByIdAsync()
            {
                throw new NotImplementedException();
            }            
        }

        private sealed class AddedState : AggregateState
        {
            private readonly UnitOfWork<TKey, TAggregate> _unitOfWork;
            private readonly TAggregate _aggregate;
            private readonly List<IEvent> _events;

            public AddedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate)               
            {
                _unitOfWork = unitOfWork;
                _aggregate = aggregate;
                _events = new List<IEvent>();
            }

            protected override UnitOfWork<TKey, TAggregate> UnitOfWork =>
                _unitOfWork;

            protected override TKey AggregateId =>
                _aggregate.Id;

            protected override bool Enter()
            {
                foreach (var @event in _aggregate.FlushEvents())
                {
                    _events.Add(@event);

                    Repository.Context.OutputStream.Publish(@event);
                }
                _aggregate.EventPublished += HandleEventPublished;
                return true;
            }

            protected override void Exit() =>
                _aggregate.EventPublished -= HandleEventPublished;

            private void HandleEventPublished(object sender, EventPublishedEventArgs e) =>
                _events.Add(e.WriteEventTo(Repository.Context.OutputStream));

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) =>
                changeSet.AddAggregateToInsert(_aggregate, _events);

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork) =>
                new UnmodifiedState(unitOfWork, _aggregate);

            public override Task<TAggregate> GetByIdAsync()
            {
                throw new NotImplementedException();
            }

            public override Task AddAsync(TAggregate aggregate)
            {
                throw new NotImplementedException();
            }

            public override Task<bool> RemoveByIdAsync()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class ModifiedState : AggregateState
        {
            private readonly UnitOfWork<TKey, TAggregate> _unitOfWork;
            private readonly TAggregate _aggregate;
            private readonly List<IEvent> _events;

            public ModifiedState(UnitOfWork<TKey, TAggregate> unitOfWork, TAggregate aggregate, IEvent @event)
            {
                _unitOfWork = unitOfWork;
                _aggregate = aggregate;
                _events = new List<IEvent>() { @event };
            }

            protected override UnitOfWork<TKey, TAggregate> UnitOfWork =>
                _unitOfWork;

            protected override TKey AggregateId =>
                _aggregate.Id;

            protected override bool Enter()
            {
                _aggregate.EventPublished += HandleEventPublished;                
                return true;
            }

            protected override void Exit() =>
                _aggregate.EventPublished -= HandleEventPublished;

            private void HandleEventPublished(object sender, EventPublishedEventArgs e) =>
                _events.Add(e.WriteEventTo(Repository.Context.OutputStream));

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) =>
                changeSet.AddAggregateToUpdate(_aggregate, _events);

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork) =>
                new UnmodifiedState(unitOfWork, _aggregate);

            public override Task<TAggregate> GetByIdAsync() =>
                AsyncMethod.Value(_aggregate);

            public override Task AddAsync(TAggregate aggregate)
            {
                throw new NotImplementedException();
            }

            public override Task<bool> RemoveByIdAsync()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class RemovedState : AggregateState
        {
            private readonly UnitOfWork<TKey, TAggregate> _unitOfWork;
            private readonly TKey _id;
            private readonly TAggregate _aggregate;

            public RemovedState(UnitOfWork<TKey, TAggregate> unitOfWork, TKey id, TAggregate aggregate)
            {
                _unitOfWork = unitOfWork;
                _id = id;
                _aggregate = aggregate;
            }

            protected override UnitOfWork<TKey, TAggregate> UnitOfWork =>
                _unitOfWork;

            protected override TKey AggregateId =>
                _id;

            protected override bool Enter()
            {
                throw new NotImplementedException();
            }

            protected override void Exit()
            {
                throw new NotImplementedException();
            }

            public override void AddToChangeSet(ChangeSet<TKey> changeSet) =>
                changeSet.AddAggregateToDelete(_id);

            protected override AggregateState CreateCommittedState(UnitOfWork<TKey, TAggregate> unitOfWork) =>
                new NullState(unitOfWork, _id);

            public override Task<TAggregate> GetByIdAsync()
            {
                throw new NotImplementedException();
            }

            public override Task AddAsync(TAggregate aggregate)
            {
                throw new NotImplementedException();
            }

            public override Task<bool> RemoveByIdAsync()
            {
                throw new NotImplementedException();
            }
        }        

        #endregion

        private readonly Repository<TKey, TAggregate> _repository;
        private readonly Dictionary<TKey, AggregateState> _aggregates;
        private bool _requiresFlush;

        public UnitOfWork(Repository<TKey, TAggregate> repository)
        {
            _repository = repository;
            _aggregates = new Dictionary<TKey, AggregateState>();
        }        

        public UnitOfWork<TKey, TAggregate> Commit()
        {
            var unitOfWork = new UnitOfWork<TKey, TAggregate>(_repository);
            var committedChanges = unitOfWork._aggregates;

            foreach (var aggregate in _aggregates)
            {
                committedChanges.Add(aggregate.Key, aggregate.Value.Commit(unitOfWork));
            }
            return unitOfWork;
        }

        /// <inheritdoc />
        public override string ToString() =>
            BuildChangeSet().ToString();

        #region [====== IRepository<TKey, TAggregate> ======]

        public Task<TAggregate> GetByIdAsync(TKey id) =>
            GetAggregateState(id).GetByIdAsync();

        public Task AddAsync(TAggregate aggregate) =>
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
            AggregateState state;

            if (_aggregates.TryGetValue(id, out state))
            {
                return state;
            }   
            return new NullState(this, id);         
        }

        #endregion

        #region [====== IUnitOfWork ======]

        public bool RequiresFlush() =>
            _requiresFlush;

        public Task FlushAsync() =>
            _repository.FlushAsync(BuildChangeSet());

        private ChangeSet<TKey> BuildChangeSet()
        {
            var changeSet = new ChangeSet<TKey>();

            foreach (var aggregate in _aggregates.Values)
            {
                aggregate.AddToChangeSet(changeSet);
            }
            return changeSet;
        }

        #endregion
    }
}
