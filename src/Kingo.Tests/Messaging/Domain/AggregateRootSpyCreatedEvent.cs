using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyCreatedAggregateEvent : AggregateEvent<Guid, int>
    {
        public AggregateRootSpyCreatedAggregateEvent(Guid id)
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
