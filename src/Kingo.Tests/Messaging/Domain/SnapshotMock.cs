using System;

namespace Kingo.Messaging.Domain
{
    public sealed class SnapshotMock : Snapshot<Guid, int>
    {
        private readonly bool _aggregateHasEventHandlers;

        public SnapshotMock(bool aggregateHasEventHandlers)
        {
            _aggregateHasEventHandlers = aggregateHasEventHandlers;
        }

        [AggregateId]
        public Guid Id
        {
            get;
            set;
        }

        [AggregateVersion]
        public int Version
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        protected override IAggregateRoot RestoreAggregate()
        {
            if (_aggregateHasEventHandlers)
            {
                return new AggregateRootWithEventHandlers(this);
            }
            return new AggregateRootWithoutEventHandlers(this);
        }
    }
}
