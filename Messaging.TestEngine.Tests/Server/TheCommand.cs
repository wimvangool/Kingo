using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal sealed class TheCommand
    {
        public Exception ExceptionToThrow;

        public IEnumerable<object> DomainEventsToPublish;
    }
}
