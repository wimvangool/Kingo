using System;

namespace Kingo.MicroServices.Domain
{
    public sealed class AggregateRootSpyCreatedEvent : Event<Guid, int>
    {
        public AggregateRootSpyCreatedEvent(Guid id) :
            base(id, 1) { }        
    }
}
