using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootWithoutEventHandlers : AggregateRootSpy
    {        
        public AggregateRootWithoutEventHandlers(bool subtractOneForNextVersion = false)
            : base(subtractOneForNextVersion) { }

        public AggregateRootWithoutEventHandlers(Guid id)
            : base(new AggregateRootSpyCreatedAggregateEvent(id)) { }

        public AggregateRootWithoutEventHandlers(AggregateRootSpyCreatedAggregateEvent aggregateEvent) :
            base(aggregateEvent) { }

        public AggregateRootWithoutEventHandlers(SnapshotMock snapshot)
            : base(snapshot) { }

        protected override ISnapshot<Guid, int> TakeSnapshot() => new SnapshotMock(false)
        {
            Id = Id,
            Version = Version,
            Value = Value
        };

        public override void ChangeValue(int newValue)
        {
            if (Value == newValue)
            {
                return;
            }
            Value = newValue;

            Publish(new ValueChangedEvent
            {
                NewValue = newValue
            });
        }
    }
}
