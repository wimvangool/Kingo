using System.Collections.Generic;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerPipeline : MessageProcessorPipeline<MessageHandlerModule>
    {
        internal MessageHandlerPipeline(IEnumerable<MessageHandlerModule> modules)
            : base(modules) { }

        internal IMessageHandlerWrapper ConnectTo(IMessageHandlerWrapper handler)
        {            
            foreach (var module in Modules)
            {
                handler = new MessageHandler(handler, module);
            }
            return handler;
        }
    }
}
