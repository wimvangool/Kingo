using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyRemovedEvent : Event<Guid, int>
    {
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
