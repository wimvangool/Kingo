using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyRemovedAggregateEvent : AggregateEvent<Guid, int>
    {
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
