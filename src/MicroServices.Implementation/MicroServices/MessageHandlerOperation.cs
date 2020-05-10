using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an operation where a <see cref="MicroProcessor"/> invokes one or more message handlers to handle a message.
    /// </summary>
    public abstract class MessageHandlerOperation : MicroProcessorOperation<MessageHandlerOperationResult>
    {        
        internal MessageHandlerOperation(CancellationToken? token)
        {            
            Token = token ?? CancellationToken.None;
        }        

        /// <inheritdoc />
        public override CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        public override MicroProcessorOperationType Type =>
            MicroProcessorOperationType.MessageHandlerOperation;

        #region [====== HandleAsync ======]

        private abstract class HandleAsyncMethod
        {
            public abstract Task<MessageHandlerOperationResult> InvokeAsync(MessageHandlerOperation operation, Message<object> message, MessageHandlerOperationContext context);
        }

        private sealed class HandleAsyncMethod<TMessage> : HandleAsyncMethod
        {
            public override Task<MessageHandlerOperationResult> InvokeAsync(MessageHandlerOperation operation, Message<object> message, MessageHandlerOperationContext context) =>
                operation.HandleAsync(message.ConvertTo<TMessage>(), context);
        }

        private static readonly ConcurrentDictionary<Type, HandleAsyncMethod> _HandleAsyncMethods = new ConcurrentDictionary<Type, HandleAsyncMethod>();

        internal Task<MessageHandlerOperationResult> HandleAsync(Message<object> message, MessageHandlerOperationContext context) =>
            _HandleAsyncMethods.GetOrAdd(message.Content.GetType(), CreateHandleAsyncMethod).InvokeAsync(this, message, context);

        private static HandleAsyncMethod CreateHandleAsyncMethod(Type messageType)
        {
            var handleAsyncMethodDefinition = typeof(HandleAsyncMethod<>);
            var handleAsyncMethod = handleAsyncMethodDefinition.MakeGenericType(messageType);
            return (HandleAsyncMethod) Activator.CreateInstance(handleAsyncMethod);
        }

        internal abstract Task<MessageHandlerOperationResult> HandleAsync<TMessage>(Message<TMessage> message, MessageHandlerOperationContext context);

        #endregion
    }
}
