namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a factory that can be used to build a <see cref="IMessageHandler{TMessage}" /> pipeline.
    /// </summary>
    /// <typeparam name="TMessage">Type of the messages that are handled by the pipeline.</typeparam>
    public interface IMessageHandlerPipelineFactory<TMessage> where TMessage : class
    {
        /// <summary>
        /// Creates and returns a pipeline of <see cref="IMessageHandler{TMessage}">MessageHandlers</see>
        /// on top of the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The handler on which the pipeline is built.</param>
        /// <returns>The created pipeline</returns>.
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        IMessageHandler<TMessage> CreateMessageHandlerPipeline(IMessageHandler<TMessage> handler);
    }
}
