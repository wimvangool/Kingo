using System;
using System.Security.Principal;
using System.Threading;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerContext : MicroProcessorContext
    {                     
        private EventStream _outputStream;        

        public MessageHandlerContext(IPrincipal principal, CancellationToken? token = null) :
            base(principal, token, new MessageStackTrace(MessageSources.InputStream))
        {            
            _outputStream = new EventStreamImplementation();            
        }        

        internal override EventStream OutputStreamCore =>
            _outputStream;             

        internal void Reset() =>
            _outputStream = new EventStreamImplementation();
    }
}
