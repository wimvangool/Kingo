using System;

namespace Kingo.Messaging.Domain
{
    public sealed class ValueChangedEvent : Event<Guid, int>
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

        public int NewValue
        {
            get;
            set;
        }
    }
}
