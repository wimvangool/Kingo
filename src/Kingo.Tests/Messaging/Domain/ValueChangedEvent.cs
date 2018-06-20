using System;

namespace Kingo.Messaging.Domain
{
    public sealed class ValueChangedEvent : Event<Guid, int>
    {        
        public ValueChangedEvent(Guid id = default(Guid), int version = 0) :
            base(id, version) { }

        public int NewValue
        {
            get;
            set;
        }
    }
}
