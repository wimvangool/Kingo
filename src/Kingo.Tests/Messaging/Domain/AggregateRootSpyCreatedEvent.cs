using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyCreatedEvent : Event<Guid, int>
    {
        public AggregateRootSpyCreatedEvent(Guid id) :
            base(id, 1) { }        
    }
}
