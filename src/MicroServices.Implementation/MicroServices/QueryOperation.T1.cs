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
            catch (OperationCanceledException exception)
            {
                if (exception.CancellationToken == Token)
                {
                    throw NewGatewayTimeoutException(exception, operation.CaptureStackTrace());
                }
                throw NewInternalServerErrorException(exception, operation.CaptureStackTrace());
            }
            catch (Exception exception)
            {
                throw NewInternalServerErrorException(exception, operation.CaptureStackTrace());
            }
        }

        private QueryOperationResult<TResponse> Commit(QueryOperationResult<TResponse> result)
        {
            try
            {
                return result;
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

        /// <summary>
        /// Creates and returns a new <see cref="ExecuteAsyncMethodOperation{TResponse}"/> for the query that is to be executed.
        /// </summary>
        /// <param name="context">Context of the operation.</param>
        /// <returns>A new operation that is ready to be executed.</returns>
        protected abstract ExecuteAsyncMethodOperation<TResponse> CreateMethodOperation(MicroProcessorOperationContext context);
    }
}
