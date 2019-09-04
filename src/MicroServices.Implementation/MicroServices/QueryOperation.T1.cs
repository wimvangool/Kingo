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
        public override MicroProcessorOperationKinds Kind =>
            Context.StackTrace.Count == 0 ? MicroProcessorOperationKinds.RootOperation : MicroProcessorOperationKinds.BranchOperation;                        

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
                // When executing a query, MessageHandlerOperationExceptions should not occur.
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
