namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents an <see cref="IQuery{TRequest, TResponse}"/>-operation that is executed by a
    /// <see cref="IMicroProcessor" /> as part of a test.
    /// </summary>
    public sealed class QueryTestOperationInfo<TRequest> : MicroProcessorTestOperationInfo
    {
        /// <summary>
        /// Gets or sets the request that will be handled by the <see cref="IQuery{TRequest, TResponse}"/>
        /// </summary>
        public TRequest Request
        {
            get;
            set;
        }
    }
}
