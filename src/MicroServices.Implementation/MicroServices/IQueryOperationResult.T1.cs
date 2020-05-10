namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of executing a query.
    /// </summary>
    public interface IQueryOperationResult<out TResponse>
    {
        /// <summary>
        /// The response of the query.
        /// </summary>
        IMessage<TResponse> Output
        {
            get;
        }
    }
}
