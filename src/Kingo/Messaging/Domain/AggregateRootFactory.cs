using System;
using System.Collections.Generic;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Messaging.Domain
{
    internal sealed class AggregateRootFactory : IAggregateRootFactory
    {
        private readonly IAggregateRootFactory _snapshotOrCreatedEvent;
        private readonly IEnumerable<IEvent> _eventsToApply;

        private AggregateRootFactory(IAggregateRootFactory snapshotOrCreatedEvent, IEnumerable<IEvent> eventsToApply)
        {
            _snapshotOrCreatedEvent = snapshotOrCreatedEvent;
            _eventsToApply = eventsToApply;
        }

        public IAggregateRoot RestoreAggregate()
        {
            if (_snapshotOrCreatedEvent == null)
            {
                throw NewMissingSnapshotOrCreatedEventException();
            }
            var aggregate = _snapshotOrCreatedEvent.RestoreAggregate();
            aggregate.LoadFromHistory(_eventsToApply);
            return aggregate;
        }

        private static Exception NewMissingSnapshotOrCreatedEventException() =>
            new NotSupportedException(ExceptionMessages.AggregateRootFactory_MissingSnapshotOrCreatedEvent);

        public override string ToString() =>
            $"<{ToString(_snapshotOrCreatedEvent)}>{ToString(_eventsToApply)}";

        private static string ToString(object snapshotOrCreatedEvent) =>
            snapshotOrCreatedEvent == null ? "?" : snapshotOrCreatedEvent.GetType().FriendlyName();

        private static string ToString(IEnumerable<object> events) =>
            string.Concat(events.Select(@event => $"+{@event.GetType().FriendlyName()}"));

        public static AggregateRootFactory FromDataSet(ISnapshot snapshot, IReadOnlyList<IEvent> events) =>
            snapshot == null
                ? new AggregateRootFactory(events.FirstOrDefault(), events.Skip(1))
                : new AggregateRootFactory(snapshot, events);
    }
}
