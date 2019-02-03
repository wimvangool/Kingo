using System;

namespace Kingo.MicroServices.Domain
{
    internal sealed class NumberUsingSnapshots : Number
    {
        #region [====== Snapshot ======]

        internal sealed class Snapshot : NumberSnapshot
        {
            public Snapshot(Guid id, int version, int value, bool hasBeenRemoved)
                : base(id, version, value, hasBeenRemoved) { }

            protected override IAggregateRoot<Guid, int> RestoreAggregate(IEventBus eventBus = null) =>
                new NumberUsingSnapshots(eventBus, this);
        }

        #endregion

        #region [====== CreatedEvent ======]

        internal sealed class CreatedEvent : NumberCreatedEvent
        {
            public CreatedEvent(Guid id, int value)
                : base(id, value) { }

            protected override IAggregateRoot<Guid, int> RestoreAggregate(IEventBus eventBus = null) =>
                new NumberUsingSnapshots(eventBus, this, false);
        }

        #endregion

        private NumberUsingSnapshots(IEventBus eventBus, NumberSnapshot snapshot)
            : base(eventBus, snapshot) { }

        private NumberUsingSnapshots(IEventBus eventBus, NumberCreatedEvent @event, bool isNewAggregate)
            : base(eventBus, @event, isNewAggregate) { }

        public override void Add(int value)
        {
            Value = Value + value;
            Publish((id, version) => new ValueAddedEvent(id, version, Value));
        }

        protected override NumberSnapshot TakeSnapshot() =>
            new Snapshot(Id, Version, Value, HasBeenRemoved);

        public static Number CreateNumber(Guid id, int value, IEventBus eventBus = null) =>
            new NumberUsingSnapshots(eventBus, new CreatedEvent(id, value), true);
    }
}
