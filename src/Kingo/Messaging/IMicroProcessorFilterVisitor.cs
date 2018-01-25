namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a visitor that visits all filter of a constructed message pipeline.
    /// </summary>
    public interface IMicroProcessorFilterVisitor
    {
        /// <summary>
        /// Visits one filter of the entire pipeline.
        /// </summary>
        /// <param name="filter">The filter to visit.</param>
        void Visit(IMicroProcessorFilter filter);

        /// <summary>
        /// Visits the <see cref="IMessageHandler{T}" /> at the end of the pipeline.
        /// </summary>        
        /// <param name="handler">The handler to visit.</param>
        void Visit<TMessage>(IMessageHandler<TMessage> handler);

        /// <summary>
        /// Visits the <see cref="IQuery{T}" /> at the end of the pipeline.
        /// </summary>        
        /// <param name="query">The query to visit.</param>
        void Visit<TMessageOut>(IQuery<TMessageOut> query);

        /// <summary>
        /// Visits the <see cref="IQuery{T, S}" /> at the end of the pipeline.
        /// </summary>        
        /// <param name="query">The query to visit.</param>
        void Visit<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query);
    }
}
