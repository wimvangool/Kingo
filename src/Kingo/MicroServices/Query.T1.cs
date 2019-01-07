namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a query as part of the <see cref="MicroProcessor" />'s pipeline.
    /// </summary>
    /// <typeparam name="TMessageOut">Type of the response that is returned by this query.</typeparam>
    public abstract class Query<TMessageOut> : MessageHandlerOrQuery, IMessageHandlerOrQuery<TMessageOut>
    {
        MicroProcessorContext IMessageHandlerOrQuery<TMessageOut>.Context =>
            Context;        

        /// <summary>
        /// Returns the context in which the query is invoked.
        /// </summary>
        public abstract QueryContext Context
        {
            get;
        }

        /// <inheritdoc />
        public abstract MessageHandlerOrQueryMethod<TMessageOut> Method
        {
            get;
        }               
    }
}
