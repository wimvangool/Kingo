using System;

namespace Kingo.MicroServices.Domain
{
    public sealed class AggregateRootWithEventHandlers : AggregateRootSpy
    {
        private readonly bool _registerEventHandlerTwice;

        public AggregateRootWithEventHandlers(bool subtractOneForNextVersion = false, bool registerEventHandlerTwice = false)
            : base(subtractOneForNextVersion)
        {
            _registerEventHandlerTwice = registerEventHandlerTwice;
        }

        public AggregateRootWithEventHandlers(AggregateRootSpyCreatedEvent aggregateEvent) :
            base(aggregateEvent, true) { }

        public AggregateRootWithEventHandlers(SnapshotMock snapshot)
            : base(snapshot) { }

        protected override ISnapshot<Guid, int> TakeSnapshot() => new SnapshotMock(true)
        {
            Id = Id,
            Version = Version,
            Value = Value
        };       

        protected override EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers)
        {
            eventHandlers = eventHandlers.Register<ValueChangedEvent>(OnValueChanged);

            if (_registerEventHandlerTwice)
            {
                return eventHandlers.Register<ValueChangedEvent>(OnValueChanged);
            }
            return eventHandlers;
        }

        public override void ChangeValue(int newValue)
        {
            if (Value == newValue)
            {
                return;
            }
            Publish(new ValueChangedEvent
            {
                NewValue = newValue
            });
        }

        private void OnValueChanged(ValueChangedEvent @event)
        {
            Value = @event.NewValue;
        }
    }
}
