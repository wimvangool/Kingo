using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{    
    internal class MessageHandlerOperation<TMessage> : MessageHandlerOperation, IMessageProcessor
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

            public override MicroProcessorOperationKinds Kind =>
                _operation.Kind;            

            public override async Task<MessageHandlerOperationResult> ExecuteAsync()
            {
                await _method.HandleAsync(_operation._message.Content, _context).ConfigureAwait(false);
                return new MessageBusResult(_context.MessageBus);
            }
        }

        #endregion

        private readonly MessageHandlerOperationContext _context;
        private readonly IMessageToProcess<TMessage> _message;                

        private MessageHandlerOperation(MessageHandlerOperationContext context, IMessageToProcess<TMessage> message) :
            this(context, message, context.StackTrace.CurrentOperation.Token) { }

        protected MessageHandlerOperation(MessageHandlerOperationContext context, IMessageToProcess<TMessage> message, CancellationToken? token) :
            base(token)
        {
            _context = context;
            _message = message;
        }

        internal MicroProcessor Processor =>
            _context.Processor;

        Task<MessageHandlerOperationResult> IMessageProcessor.HandleAsync<TEvent>(IMessageToProcess<TEvent> @event, MessageHandlerOperationContext context) =>
            new MessageHandlerOperation<TEvent>(context, @event).ExecuteAsync();

        public override IMessageToProcess Message =>
            _message;

        public override MicroProcessorOperationKinds Kind =>
            MicroProcessorOperationKinds.BranchOperation;        

        public override async Task<MessageHandlerOperationResult> ExecuteAsync()
        {
            var result = MessageBufferResult.Empty;

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
                // Every operation potentially yields a new stream of events, which is immediately handled by the processor
                // inside the current context. The processor uses a depth-first approach, which means that each event and its resulting
                // sub-tree of events are handled before the next event in the stream.
                var result = await operation.ExecuteAsync().ConfigureAwait(false);
                if (result.Messages.Count > 0)
                {
                    return await HandleEventsAsync(result.ToMessageBufferResult(), operation.Context).ConfigureAwait(false);
                }
                return result;
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }            
        }

        private async Task<MessageHandlerOperationResult> HandleEventsAsync(MessageBufferResult result, MessageHandlerOperationContext context) =>
            result.Append(await result.MessageBuffer.HandleEventsWith(this, context).ConfigureAwait(false));

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
                foreach (var method in methodFactory.CreateMethodsFor<TMessage>(Kind, Processor.ServiceProvider))
                {
                    yield return CreateMethodOperation(method, context);
                }
            }
        }

        protected HandleAsyncMethodOperation CreateMethodOperation(HandleAsyncMethod<TMessage> method, MessageHandlerOperationContext context) =>
            new MethodOperation(this, method, context);
    }
}
