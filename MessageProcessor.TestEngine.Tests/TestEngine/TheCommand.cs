using System;
using System.Collections.Generic;

namespace YellowFlare.MessageProcessing.TestEngine
{
    internal sealed class TheCommand
    {
        public Exception ExceptionToThrow;

        public IEnumerable<object> DomainEventsToPublish;
    }
}
