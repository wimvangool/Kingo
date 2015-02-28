using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerPipeline : MessageProcessorPipeline<IMessageHandlerModule>
    {
        internal MessageHandlerPipeline(IEnumerable<IMessageHandlerModule> modules)
            : base(modules) { }

        internal IMessageHandler ConnectTo(IMessageHandler handler)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            foreach (var module in Modules)
            {
                handler = new MessageHandler(handler, module);
            }
            return handler;
        }
    }
}
