using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    internal sealed class TheCommand : Message
    {
        public Exception ExceptionToThrow;

        public IEnumerable<DomainEvent> DomainEventsToPublish;

        public override Message Copy()
        {
            return new TheCommand
            {
                ExceptionToThrow = ExceptionToThrow,
                DomainEventsToPublish = DomainEventsToPublish
            };
        }
    }
}
