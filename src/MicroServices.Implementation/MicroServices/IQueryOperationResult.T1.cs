namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of executing a query.
    /// </summary>
    public interface IQueryOperationResult<TResponse>
    {
        /// <summary>
        /// The response of the query.
        /// </summary>
        MessageEnvelope<TResponse> Output
        {
            get;
        }
    }
}
