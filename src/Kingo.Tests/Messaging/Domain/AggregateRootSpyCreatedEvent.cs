using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyCreatedEvent : Event<Guid, int>
    {
        public AggregateRootSpyCreatedEvent(Guid id)
        {
            Id = id;
            Version = 1;
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
    }
}
