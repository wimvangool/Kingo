using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyCreatedEvent : Event<Guid, int>
    {
        public AggregateRootSpyCreatedEvent(Guid id)
        {
            Id = id;
        }

        public override Guid Id
        {
            get;
            set;
        }

        public override int Version
        {
            get;
            set;
        }
    }
}
