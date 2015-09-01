using System.Collections.Generic;

namespace ServiceComponents.ComponentModel.Server
{
    internal sealed class MessageHandlerPipeline : MessageProcessorPipeline<MessageHandlerModule>
    {
        internal MessageHandlerPipeline(IEnumerable<MessageHandlerModule> modules)
            : base(modules) { }

        internal IMessageHandler ConnectTo(IMessageHandler handler)
        {            
            foreach (var module in Modules)
            {
                handler = new MessageHandler(handler, module);
            }
            return handler;
        }
    }
}
