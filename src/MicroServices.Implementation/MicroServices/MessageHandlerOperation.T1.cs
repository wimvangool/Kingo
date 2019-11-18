using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{    
    internal class MessageHandlerOperation<TMessage> : MessageHandlerOperation
    {
        #region [====== MethodOperation ======]

        private sealed class MethodOperation : HandleAsyncMethodOperation
        {
            private readonly MessageHandlerOperation<TMessage> _operation;
            private readonly HandleAsyncMethod<TMessage> _method;            
            private readonly MessageHandlerOperationContext _context;            

            public MethodOperation(MessageHandlerOperation<TMessage> operation, HandleAsyncMethod<TMessage> method, MessageHandlerOperationContext context)
            {
                _operation = operation;
                _method = method;                
                _context = context.PushOperation(this);                
            }

            public override HandleAsyncMethod Method =>
                _method;

            public override IMessageToProcess Message =>
                _operation._message;

            public override MessageHandlerOperationContext Context =>
                _context;

            public override CancellationToken Token =>
                _operation.Token;

            public override MicroProcessorOperationKind Kind =>
                _operation.Kind;            

            public override async Task<MessageHandlerOperationResult> ExecuteAsync()
            {
                await _method.HandleAsync(_operation._message.Content, _context).ConfigureAwait(false);
                return new MessageBusResult(_context.MessageBus);
            }
        }

        #endregion

        private readonly MessageHandlerOperationContext _context;
        private readonly MessageToProcess<TMessage> _message;
        private readonly bool _isScheduledEvent;

        private MessageHandlerOperation(MessageHandlerOperationContext context, MessageToDispatch<TMessage> message) :
            this(context, message.ToProcess(), context.StackTrace.CurrentOperation.Token)
        {
            _isScheduledEvent = message.DeliveryTimeUtc.HasValue;
        }

        protected MessageHandlerOperation(MessageHandlerOperationContext context, MessageToProcess<TMessage> message, CancellationToken? token) :
            base(token)
        {
            _context = context;
            _message = message;
        }

        internal MicroProcessor Processor =>
            _context.Processor;

        internal override Task<MessageHandlerOperationResult> HandleAsync<TEvent>(MessageToDispatch<TEvent> message, MessageHandlerOperationContext context) =>
            new MessageHandlerOperation<TEvent>(context, message).ExecuteAsync();

        public override IMessageToProcess Message =>
            _message;

        public override MicroProcessorOperationKind Kind =>
            MicroProcessorOperationKind.BranchOperation;        

        public override async Task<MessageHandlerOperationResult> ExecuteAsync()
        {
            var result = MessageHandlerOperationResult.Empty;

            foreach (var operation in CreateMethodOperationPipelines(_context))
            {
                result = result.Append(await ExecuteAsync(operation).ConfigureAwait(false));
            }
            return result;
        }

        private async Task<MessageHandlerOperationResult> ExecuteAsync(HandleAsyncMethodOperation operation)
        {
            Token.ThrowIfCancellationRequested();

            try
            {
                // After the operation has been executed, its result is committed, which means no more messages
                // can be added/produced by the operation and all messages are automatically correlated to the
                // current message.
                var result = Commit(await operation.ExecuteAsync().ConfigureAwait(false), operation);
                if (result.Output.Count > 0)
                {
                    // Every operation potentially yields a new stream of messages, which is immediately handled by the processor
                    // inside the current context. The processor uses a depth-first approach, which means that each message and its resulting
                    // sub-tree of events are handled before the next event in the stream.
                    return result.Append(await HandleAsync(result.Output, operation.Context).ConfigureAwait(false));
                }
                return result;
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }            
        }

        private static MessageHandlerOperationResult Commit(MessageHandlerOperationResult result, IMicroProcessorOperation operation) =>
            result.Commit(operation.Message);

        private async Task<MessageHandlerOperationResult> HandleAsync(IEnumerable<MessageToDispatch> messages, MessageHandlerOperationContext context)
        {
            var result = MessageHandlerOperationResult.Empty;

            foreach (var message in messages)
            {
                result = result.Append(await HandleAsync(message, context).ConfigureAwait(false));
            }
            return result;
        }

        private IEnumerable<HandleAsyncMethodOperation> CreateMethodOperationPipelines(MessageHandlerOperationContext context)
        {
            // TODO
            return CreateMethodOperations(context);
        }

        protected virtual IEnumerable<HandleAsyncMethodOperation> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            var methodFactory = Processor.ServiceProvider.GetService<IHandleAsyncMethodFactory>();
            if (methodFactory != null)
            {
                foreach (var method in methodFactory.CreateInternalEventBusEndpointsFor<TMessage>(Processor.ServiceProvider, _isScheduledEvent))
                {
                    yield return CreateMethodOperation(method, context);
                }
            }
        }

        protected HandleAsyncMethodOperation CreateMethodOperation(HandleAsyncMethod<TMessage> method, MessageHandlerOperationContext context) =>
            new MethodOperation(this, method, context);
    }
}
