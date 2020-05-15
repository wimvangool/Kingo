using System.Collections.Generic;
using System.Threading;

namespace Kingo.MicroServices
{    
    internal sealed class CommandHandlerBranchOperation<TMessage> : MessageHandlerOperation<TMessage>
    {
        private readonly HandleAsyncMethod<TMessage> _method;

        internal CommandHandlerBranchOperation(MessageHandlerOperationContext context, IMessageHandler<TMessage> messageHandler, Message<TMessage> message, CancellationToken? token) :
            base(context, message, token)
        {
            _method = new HandleAsyncMethod<TMessage>(messageHandler);
        }

        protected override IEnumerable<HandleAsyncMethodOperation<TMessage>> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            yield return CreateMethodOperation(_method, context);
        }
    }
}
