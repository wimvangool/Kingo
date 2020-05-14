using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices
{    
    public class MessageHandlerOperation<TMessage> : MicroProcessorOperation<MessageHandlerOperationResult<TMessage>>
    {
        #region [====== MethodOperation ======]

        private sealed class MethodOperation : HandleAsyncMethodOperation<TMessage>
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

            public override async Task<MessageHandlerOperationResult<TMessage>> ExecuteAsync()
            {
                var input = Validate(ref _operation._message);
                await _method.HandleAsync(input.Content, _context).ConfigureAwait(false);
                return Validate(_context.CommitResult(input));
            }

            private Message<TMessage> Validate(ref Message<TMessage> input) =>
                Interlocked.Exchange(ref input, input.Validate(_operation.Processor.ServiceProvider));

            private MessageHandlerOperationResult<TMessage> Validate(MessageHandlerOperationResult<TMessage> output) =>
                output.Validate(_operation.Processor.ServiceProvider);
        }

        #endregion

        private readonly MessageHandlerOperationContext _context;
        private readonly CancellationToken _token;
        private Message<TMessage> _message;

        internal MessageHandlerOperation(MessageHandlerOperationContext context, Message<TMessage> message) :
            this(context, message, context.StackTrace.CurrentOperation.Token) { }

        internal MessageHandlerOperation(MessageHandlerOperationContext context, Message<TMessage> message, CancellationToken? token)
        {
            _context = context;
            _token = token ?? CancellationToken.None;
            _message = message;
        }

        internal MicroProcessor Processor =>
            _context.Processor;

        public override CancellationToken Token =>
            _token;

        public override IMessage Message =>
            _message;

        public override MicroProcessorOperationType Type =>
            MicroProcessorOperationType.MessageHandlerOperation;

        public override MicroProcessorOperationKind Kind =>
            MicroProcessorOperationKind.BranchOperation;

        public override async Task<MessageHandlerOperationResult<TMessage>> ExecuteAsync()
        {
            var result = MessageHandlerOperationResult.FromInput(_message);

            foreach (var operation in CreateMethodOperationPipelines(_context))
            {
                result = result.Append(await ExecuteAsync(operation).ConfigureAwait(false));
            }
            return result;
        }

        internal virtual async Task<MessageHandlerOperationResult<TMessage>> ExecuteAsync(HandleAsyncMethodOperation<TMessage> operation)
        {
            try
            {
                return await ExecuteOperationAsync(operation).ConfigureAwait(false);
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

        private async Task<MessageHandlerOperationResult<TMessage>> ExecuteOperationAsync(HandleAsyncMethodOperation<TMessage> operation)
        {
            Token.ThrowIfCancellationRequested();

            try
            {
                // After the operation has been executed, its result is committed, which means no more messages
                // can be added/produced by the operation and all messages are automatically correlated to the
                // current message.
                var result = await operation.ExecuteAsync().ConfigureAwait(false);
                if (result.Output.Count > 0)
                {
                    // Every operation potentially yields a new stream of events, which are immediately handled by the processor
                    // inside the current context. The processor uses a depth-first approach, which means that each events and its resulting
                    // sub-tree of events are handled before the next event in the stream.
                    return await EventProcessor.HandleEventsAsync(result, operation.Context);
                }
                return result;
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }
        }

        private IEnumerable<HandleAsyncMethodOperation<TMessage>> CreateMethodOperationPipelines(MessageHandlerOperationContext context)
        {
            // TODO
            return CreateMethodOperations(context);
        }

        protected virtual IEnumerable<HandleAsyncMethodOperation<TMessage>> CreateMethodOperations(MessageHandlerOperationContext context)
        {
            var endpointFactory = Processor.ServiceProvider.GetService<IMessageBusEndpointFactory>();
            if (endpointFactory != null)
            {
                foreach (var method in endpointFactory.CreateInternalEventBusEndpoints<TMessage>(Processor.ServiceProvider))
                {
                    yield return CreateMethodOperation(method, context);
                }
            }
        }

        internal HandleAsyncMethodOperation<TMessage> CreateMethodOperation(HandleAsyncMethod<TMessage> method, MessageHandlerOperationContext context) =>
            new MethodOperation(this, method, context);
    }
}
