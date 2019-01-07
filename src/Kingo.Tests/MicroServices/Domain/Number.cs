using System;

namespace Kingo.MicroServices.Domain
{
    public abstract class Number : AggregateRoot<Guid, int>
    {                             
        protected Number(IEventBus eventBus, NumberSnapshot snapshot)
            : base(eventBus, snapshot, false)
        {
            Value = snapshot.Value;
            HasBeenRemoved = snapshot.HasBeenRemoved;            
        }

        protected Number(IEventBus eventBus, NumberCreatedEvent @event, bool isNewAggregate)
            : base(eventBus, @event, isNewAggregate)
        {
            Value = @event.Value;            
        }

        protected override int NextVersion() =>
            Version + 1;

        protected int Value
        {
            get;
            set;
        }

        public bool EnableSoftDelete
        {
            get;
            set;
        }

        public abstract void Add(int value);

        protected override bool OnRemoved()
        {            
            Publish((id, version) => new NumberDeletedEvent(id, version));
            return !EnableSoftDelete;
        }
    }
}
