using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Domain
{
    internal sealed class NumberUsingEvents : Number
    {
        #region [====== Snapshot ======]

        private sealed class Snapshot : NumberSnapshot
        {
            public Snapshot(Guid id, int version, int value)
                : base(id, version, value) { }

            protected override IAggregateRoot<Guid, int> RestoreAggregate(IEventBus eventBus = null) =>
                new NumberUsingEvents(eventBus, this);
        }

        #endregion

        #region [====== CreatedEvent ======]

        private sealed class CreatedEvent : NumberCreatedEvent
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

        protected override EventHandlerCollection RegisterEventHandlers(EventHandlerCollection eventHandlers) =>
            eventHandlers.Register<ValueAddedEvent>(Add);

        public override void Add(int value) =>
            Publish((id, version) => new ValueAddedEvent(id, version, Value + value));

        private void Add(ValueAddedEvent @event) =>
            Value = @event.Value;

        public static Number CreateNumber(Guid id, int value, IEventBus eventBus = null) =>
            new NumberUsingEvents(eventBus, new CreatedEvent(id, value), true);
    }
}
