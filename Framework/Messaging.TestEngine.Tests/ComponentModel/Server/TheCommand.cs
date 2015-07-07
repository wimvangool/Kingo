using System;
using System.Collections.Generic;

namespace Syztem.ComponentModel.Server
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
