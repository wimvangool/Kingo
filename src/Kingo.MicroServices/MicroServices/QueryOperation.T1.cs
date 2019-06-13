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
        internal QueryOperation(MicroProcessor processor, CancellationToken? token)
        {
            Context = new QueryOperationContext(processor);
            Token = token ?? CancellationToken.None;
        }

        internal MicroProcessor Processor =>
            Context.Processor;            
        
        private QueryOperationContext Context
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
        public override MicroProcessorOperationKinds Kind =>
            MicroProcessorOperationKinds.RootOperation;

        /// <inheritdoc />
        public override async Task<QueryOperationResult<TResponse>> ExecuteAsync()
        {
            Token.ThrowIfCancellationRequested();

            try
            {
                return await CreateQueryOperationPipeline(Context).ExecuteAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                if (exception.CancellationToken == Token)
                {
                    throw;
                }
                throw InternalServerErrorException.FromInnerException(exception);
            }
            catch (MicroProcessorOperationException)
            {
                throw;
            }
            catch (MessageHandlerOperationException exception)
            {
                // When executing a query, MessageHandlerExceptions should not occur.
                // If they do, they are treated as internal server errors.
                throw exception.AsInternalServerErrorException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new InternalServerErrorException(exception.Message, exception);
            }
            finally
            {
                Token.ThrowIfCancellationRequested();
            }
        }

        private ExecuteAsyncMethodOperation<TResponse> CreateQueryOperationPipeline(QueryOperationContext context)
        {
            // TODO: Create pipeline...
            return CreateMethodOperation(context);
        }

        /// <summary>
        /// Creates and returns a new <see cref="ExecuteAsyncMethodOperation{TResponse}"/> for the query that is to be executed.
        /// </summary>
        /// <param name="context">Context of the operation.</param>
        /// <returns>A new operation that is ready to be executed.</returns>
        protected abstract ExecuteAsyncMethodOperation<TResponse> CreateMethodOperation(QueryOperationContext context);
    }
}
