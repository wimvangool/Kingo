using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an operation where a <see cref="MicroProcessor"/> executes a query.
    /// </summary>
    /// <typeparam name="TRequest">Type of the request of the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public sealed class QueryOperation<TRequest, TResponse> : MicroProcessorOperation<QueryOperationResult<TRequest, TResponse>>
    {
        #region [====== MethodOperation ======]

        private sealed class MethodOperation : ExecuteAsyncMethodOperation<TRequest, TResponse>
        {
            private readonly QueryOperation<TRequest, TResponse> _operation;
            private readonly ExecuteAsyncMethod<TRequest, TResponse> _method;
            private readonly QueryOperationContext _context;

            public MethodOperation(QueryOperation<TRequest, TResponse> operation, MicroProcessorOperationContext context)
            {
                _operation = operation;
                _method = new ExecuteAsyncMethod<TRequest, TResponse>(operation._query);
                _context = context.PushOperation(this);
            }

            public override IMessage Message =>
                _operation.Message;

            public override CancellationToken Token =>
                _operation.Token;

            public override MicroProcessorOperationType Type =>
                _operation.Type;

            public override MicroProcessorOperationKind Kind =>
                _operation.Kind;

            public override ExecuteAsyncMethod Method =>
                _method;

            public override QueryOperationContext Context =>
                _context;

            public override async Task<QueryOperationResult<TRequest, TResponse>> ExecuteAsync()
            {
                var input = Validate(ref _operation._message);
                var output = await ExecuteMethodAsync(input).ConfigureAwait(false);
                return Validate(new QueryOperationResult<TRequest, TResponse>(input, output));
            }

            private async Task<Message<TResponse>> ExecuteMethodAsync(IMessage<TRequest> message) =>
                CreateResponseMessage(await _method.ExecuteAsync(message.Content, _context).ConfigureAwait(false));

            private Message<TResponse> CreateResponseMessage(TResponse response) =>
                Context.Processor.MessageFactory.CreateResponse(MessageDirection.Output, MessageHeader.Unspecified, response);

            private Message<TRequest> Validate(ref Message<TRequest> input) =>
                Interlocked.Exchange(ref input, input.Validate(_operation.Processor.ServiceProvider));

            private QueryOperationResult<TRequest, TResponse> Validate(QueryOperationResult<TRequest, TResponse> output) =>
                output.Validate(_operation.Processor.ServiceProvider);
        }

        #endregion

        private readonly MicroProcessorOperationContext _context;
        private readonly CancellationToken _token;

        private readonly Query<TRequest, TResponse> _query;
        private Message<TRequest> _message;

        internal QueryOperation(MicroProcessor processor, Query<TRequest, TResponse> query, Message<TRequest> message, CancellationToken? token) :
            this(new QueryOperationContext(processor), query, message, token) { }

        internal QueryOperation(MicroProcessorOperationContext context, Query<TRequest, TResponse> query, Message<TRequest> message, CancellationToken? token)
        {
            _context = context;
            _token = token ?? CancellationToken.None;
            _query = query;
            _message = message;
        }

        internal MicroProcessor Processor =>
            _context.Processor;

        public override IMessage Message =>
            _message;

        /// <inheritdoc />
        public override CancellationToken Token =>
            _token;

        /// <inheritdoc />
        public override MicroProcessorOperationType Type =>
            MicroProcessorOperationType.QueryOperation;

        /// <inheritdoc />
        public override MicroProcessorOperationKind Kind =>
            _context.StackTrace.Count == 0 ? MicroProcessorOperationKind.RootOperation : MicroProcessorOperationKind.BranchOperation;

        /// <inheritdoc />
        public override Task<QueryOperationResult<TRequest, TResponse>> ExecuteAsync() =>
            ExecuteAsync(CreateQueryOperationPipeline(_context));

        private async Task<QueryOperationResult<TRequest, TResponse>> ExecuteAsync(ExecuteAsyncMethodOperation<TRequest, TResponse> operation)
        {
            try
            {
                return await ExecuteOperationAsync(operation).ConfigureAwait(false);
            }
            catch (MicroProcessorOperationException)
            {
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
                // deliberately cancelled, because this typically happens because an upstream timeout has occurred.
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

        private async Task<QueryOperationResult<TRequest, TResponse>> ExecuteOperationAsync(ExecuteAsyncMethodOperation<TRequest, TResponse> operation)
        {
            Token.ThrowIfCancellationRequested();

            try
            {
                return await operation.ExecuteAsync().ConfigureAwait(false);
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }
        }

        private ExecuteAsyncMethodOperation<TRequest, TResponse> CreateQueryOperationPipeline(MicroProcessorOperationContext context)
        {
            // TODO: Create pipeline...
            return CreateMethodOperation(context);
        }

        private ExecuteAsyncMethodOperation<TRequest, TResponse> CreateMethodOperation(MicroProcessorOperationContext context) =>
            new MethodOperation(this, context);
    }
}
