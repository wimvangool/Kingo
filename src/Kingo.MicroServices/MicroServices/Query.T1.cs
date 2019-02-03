namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a query as part of the <see cref="MicroProcessor" />'s pipeline.
    /// </summary>
    /// <typeparam name="TResponse">Type of the response that is returned by this query.</typeparam>
    public abstract class Query<TResponse> : MessageHandlerOrQuery, IMessageHandlerOrQuery<TResponse>
    {
        MicroProcessorContext IMessageHandlerOrQuery<TResponse>.Context =>
            Context;        

        /// <summary>
        /// Returns the context in which the query is invoked.
        /// </summary>
        public abstract QueryContext Context
        {
            get;
        }

        /// <inheritdoc />
        public abstract MessageHandlerOrQueryMethod<TResponse> Method
        {
            get;
        }               
    }
}
