namespace Kingo.MicroServices
{
    /// <summary>    
    /// Represents an operation of a <see cref="IMicroProcessor"/> where a query ie being executed.    
    /// </summary>
    /// <typeparam name="TRequest">Type of the request of the query.</typeparam>
    /// <typeparam name="TResponse">Type of the response of the query.</typeparam>
    public abstract class ExecuteAsyncMethodOperation<TRequest, TResponse> : MicroProcessorOperation<QueryOperationResult<TRequest, TResponse>>, IAsyncMethodOperation<QueryOperationResult<TRequest, TResponse>>
    {
        #region [====== IAsyncMethodOperation ======]

        IAsyncMethod IAsyncMethodOperation.Method =>
            Method;

        /// <summary>
        /// Returns the <see cref="IQuery{TResponse}.ExecuteAsync"/> or <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/>
        /// method that is being invoked in this operation.
        /// </summary>
        public abstract ExecuteAsyncMethod Method
        {
            get;
        }        

        MicroProcessorOperationContext IAsyncMethodOperation.Context =>
            Context;

        /// <summary>
        /// Returns the context of this operation.
        /// </summary>
        public abstract QueryOperationContext Context
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            ToString(Method);

        #endregion

        #region [====== IMicroProcessorOperation<ExecuteAsyncResult<TRequest, TResponse>> ======]

        /// <inheritdoc />
        public AsyncMethodOperation<QueryOperationResult<TRequest, TResponse>> ToAsyncMethodOperation() =>
            new AsyncMethodOperation<QueryOperationResult<TRequest, TResponse>>(this);        

        #endregion
    }
}
