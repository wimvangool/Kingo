using System;
using System.Collections.Generic;

namespace Kingo.Messaging
{
    internal sealed class TheCommand : Message<TheCommand>
    {
        public Exception ExceptionToThrow;

        public IEnumerable<DomainEvent> DomainEventsToPublish;

        public override TheCommand Copy()
        {
            return new TheCommand()
            {
                ExceptionToThrow = ExceptionToThrow,
                DomainEventsToPublish = DomainEventsToPublish
            };
        }
    }
}
