using System;

namespace Kingo.Messaging.Domain
{
    public sealed class ValueChangedEvent : AggregateEvent<Guid, int>
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

        public int NewValue
        {
            get;
            set;
        }
    }
}
