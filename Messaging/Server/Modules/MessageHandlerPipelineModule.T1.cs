namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Serves as a base class for modules that are part of a <see cref="IMessageHandler{TMessage}" />-pipeline.
    /// </summary>
    /// <typeparam name="TMessage">Type of message to handle.</typeparam>
    public abstract class MessageHandlerPipelineModule<TMessage> : MessageHandler<TMessage> where TMessage : class
    {
        /// <summary>
        /// Returns the next <see cref="IMessageHandler{TMessage}" /> to invoke.
        /// </summary>
        protected abstract IMessageHandler<TMessage> Handler
        {
            get;
        }        
    }
}
