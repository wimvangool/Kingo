using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal abstract class Event : MessageType, IMessage, IEventBuffer
    {
        protected Event(Type type) :
            base(type, MessageKind.Event) { }

        public abstract object Instance
        {
            get;
        }

        public abstract Task<MessageHandlerOperationResult> HandleWith(IMessageProcessor processor, MessageHandlerOperationContext context);        
    }
}
