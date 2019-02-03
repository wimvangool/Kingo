using System;

namespace Kingo.MicroServices.Domain
{
    internal sealed class NumberUsingEvents : Number
    {
        #region [====== Snapshot ======]

        internal sealed class Snapshot : NumberSnapshot
        {
            public Snapshot(Guid id, int version, int value, bool hasBeenRemoved)
                : base(id, version, value, hasBeenRemoved) { }

            protected override IAggregateRoot<Guid, int> RestoreAggregate(IEventBus eventBus = null) =>
                new NumberUsingEvents(eventBus, this);
        }

        #endregion

        #region [====== CreatedEvent ======]

        internal sealed class CreatedEvent : NumberCreatedEvent
        {
            public CreatedEvent(Guid id, int value)
                : base(id, value) { }

            protected override IAggregateRoot<Guid, int> RestoreAggregate(IEventBus eventBus = null) =>
                new NumberUsingEvents(eventBus, this, false);
        }

        #endregion

        private NumberUsingEvents(IEventBus eventBus, NumberSnapshot snapshot)
            : base(eventBus, snapshot) { }

        private NumberUsingEvents(IEventBus eventBus, NumberCreatedEvent @event, bool isNewAggregate)
            : base(eventBus, @event, isNewAggregate) { }

        protected override EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers)
        {
            return eventHandlers
                .Register<ValueAddedEvent>(Handle)
                .Register<NumberDeletedEvent>(Handle);
        }            

        public override void Add(int value) =>
            Publish((id, version) => new ValueAddedEvent(id, version, Value + value));

        private void Handle(ValueAddedEvent @event) =>
            Value = @event.Value;

        private void Handle(NumberDeletedEvent @event) =>
            HasBeenRemoved = true;

        protected override NumberSnapshot TakeSnapshot() =>
            new Snapshot(Id, Version, Value, HasBeenRemoved);

        public static Number CreateNumber(Guid id, int value, IEventBus eventBus = null) =>
            new NumberUsingEvents(eventBus, new CreatedEvent(id, value), true);
    }
}
