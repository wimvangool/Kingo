using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing
{
    internal sealed class TheCommand
    {
        public Exception ExceptionToThrow;

        public IEnumerable<object> DomainEventsToPublish;
    }
}
