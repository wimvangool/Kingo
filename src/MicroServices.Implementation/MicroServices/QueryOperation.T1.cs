using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an operation where a <see cref="MicroProcessor"/> executes a query.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public abstract class QueryOperation<TResponse> : MicroProcessorOperation<QueryOperationResult<TResponse>>
    {
        internal QueryOperation(MicroProcessor processor, CancellationToken? token) :
            this(new QueryOperationContext(processor), token) { }

        internal QueryOperation(MicroProcessorOperationContext context, CancellationToken? token)
        {
            Context = context;
            Token = token ?? CancellationToken.None;
        }

        internal MicroProcessor Processor =>
            Context.Processor;            
        
        private MicroProcessorOperationContext Context
        {
            get;
        }

        /// <inheritdoc />
        public override CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        public override MicroProcessorOperationType Type =>
            MicroProcessorOperationType.QueryOperation;

        /// <inheritdoc />
        public override MicroProcessorOperationKind Kind =>
            Context.StackTrace.Count == 0 ? MicroProcessorOperationKind.RootOperation : MicroProcessorOperationKind.BranchOperation;

        /// <inheritdoc />
        public override Task<QueryOperationResult<TResponse>> ExecuteAsync() =>
            ExecuteAsync(CreateQueryOperationPipeline(Context));

        private async Task<QueryOperationResult<TResponse>> ExecuteAsync(ExecuteAsyncMethodOperation<TResponse> operation)
        {
            Token.ThrowIfCancellationRequested();

            try
            {
                return Commit(await operation.ExecuteAsync().ConfigureAwait(false));
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

        private QueryOperationResult<TResponse> Commit(QueryOperationResult<TResponse> result)
        {
            try
            {
                return result.Commit(Message, Processor.ServiceProvider);
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }
        }

        private ExecuteAsyncMethodOperation<TResponse> CreateQueryOperationPipeline(MicroProcessorOperationContext context)
        {
            // TODO: Create pipeline...
            return CreateMethodOperation(context);
        }

        internal abstract ExecuteAsyncMethodOperation<TResponse> CreateMethodOperation(MicroProcessorOperationContext context);
    }
}
