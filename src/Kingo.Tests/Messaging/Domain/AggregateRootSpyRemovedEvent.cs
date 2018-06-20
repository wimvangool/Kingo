using System;

namespace Kingo.Messaging.Domain
{
    public sealed class AggregateRootSpyRemovedEvent : Event<Guid, int> { }
}
