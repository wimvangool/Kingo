using System;
using System.Collections.Generic;
using System.Linq;
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

            public override IMessage Message =>
                _operation._message;

            public override MessageHandlerOperationContext Context =>
                _context;

            public override CancellationToken Token =>
                _operation.Token;

            public override MicroProcessorOperationKind Kind =>
                _operation.Kind;            

            public override async Task<MessageHandlerOperationResult> ExecuteAsync()
            {
                await _method.HandleAsync(_operation.MessageContent(), _context).ConfigureAwait(false);
                return _context.MessageBusResult();
            }
        }

        #endregion

        private readonly MessageHandlerOperationContext _context;
        private Message<TMessage> _message;

        private MessageHandlerOperation(MessageHandlerOperationContext context, Message<TMessage> message) :
            this(context, message, context.StackTrace.CurrentOperation.Token) { }

        protected MessageHandlerOperation(MessageHandlerOperationContext context, Message<TMessage> message, CancellationToken? token) :
            base(token)
        {
            _context = context;
            _message = message;
        }

        internal MicroProcessor Processor =>
            _context.Processor;

        internal override Task<MessageHandlerOperationResult> HandleAsync<TEvent>(Message<TEvent> message, MessageHandlerOperationContext context) =>
            new MessageHandlerOperation<TEvent>(context, message).ExecuteAsync();

        public override IMessage Message =>
            _message;

        private TMessage MessageContent() =>
            Validate(ref _message, Processor.ServiceProvider);

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

        protected virtual async Task<MessageHandlerOperationResult> ExecuteAsync(HandleAsyncMethodOperation operation)
        {
            Token.ThrowIfCancellationRequested();

            try
            {
                // After the operation has been executed, its result is committed, which means no more messages
                // can be added/produced by the operation and all messages are automatically correlated to the
                // current message.
                var result = Commit(await operation.ExecuteAsync().ConfigureAwait(false), operation.Message);
                if (result.Messages.Count > 0)
                {
                    // Every operation potentially yields a new stream of messages, which is immediately handled by the processor
                    // inside the current context. The processor uses a depth-first approach, which means that each message and its resulting
                    // sub-tree of events are handled before the next event in the stream.
                    return result.Append(await HandleAsync(result.Messages, operation.Context).ConfigureAwait(false));
                }
                return result;
            }
            catch (MicroProcessorOperationException)
            {
                // MicroProcessorOperations are let through by default, because these exceptions
                // contain the error information that the client of the processor will want to use
                // to properly handle the exception.
                throw;
            }
            catch (InternalOperationException exception)
            {
                // When a InternalOperationException was thrown, the processor converts it into the appropriate
                // MicroProcessorOperationException, depending on the cause and current stack-trace.
                throw exception.ToMicroProcessorOperationException(operation.Context.CaptureOperationStackTrace());
            }
            catch (OperationCanceledException exception)
            {
                // OperationCanceledExceptions are treated as InternalServerErrors, even if the operation was
                // deliberately cancelled, because this typically happens when an upstream timeout has occurred.
                if (exception.CancellationToken == Token)
                {
                    throw operation.Context.NewGatewayTimeoutException(ExceptionMessages.MicroProcessorOperation_OperationCancelled, exception);
                }
                throw operation.Context.NewInternalServerErrorException(ExceptionMessages.MicroProcessorOperation_OperationCancelledUnexpectedly, exception);
            }
            catch (Exception exception)
            {
                throw operation.Context.NewInternalServerErrorException(ExceptionMessages.MicroProcessorOperation_InternalServerError, exception);
            }
        }

        private MessageHandlerOperationResult Commit(MessageHandlerOperationResult result, IMessage message)
        {
            try
            {
                return result.Commit(message);
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }
        }

        private async Task<MessageHandlerOperationResult> HandleAsync(IEnumerable<Message<object>> messages, MessageHandlerOperationContext context)
        {
            var result = MessageHandlerOperationResult.Empty;

            foreach (var message in messages.Where(IsUnscheduledEvent))
            {
                result = result.Append(await HandleAsync(message, context).ConfigureAwait(false));
            }
            return result;
        }

        private static bool IsUnscheduledEvent(IMessage message) =>
            message.Kind == MessageKind.Event && message.DeliveryTimeUtc == null;

        private IEnumerable<HandleAsyncMethodOperation> CreateMethodOperationPipelines(MessageHandlerOperationContext context)
        {
            // TODO
            return CreateMethodOperations(context);
        }

        protected virtual IEnumerable<HandleAsyncMethodOperation> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            var methodFactory = Processor.ServiceProvider.GetService<IMessageBusEndpointFactory>();
            if (methodFactory != null)
            {
                foreach (var method in methodFactory.CreateInternalEventBusEndpoints<TMessage>(Processor.ServiceProvider))
                {
                    yield return CreateMethodOperation(method, context);
                }
            }
        }

        protected HandleAsyncMethodOperation CreateMethodOperation(HandleAsyncMethod<TMessage> method, MessageHandlerOperationContext context) =>
            new MethodOperation(this, method, context);
    }
}
