using System.Collections.Generic;

namespace System.ComponentModel.Messaging.Server
{
    internal sealed class TheCommand
    {
        public Exception ExceptionToThrow;

        public IEnumerable<object> DomainEventsToPublish;
    }
}
