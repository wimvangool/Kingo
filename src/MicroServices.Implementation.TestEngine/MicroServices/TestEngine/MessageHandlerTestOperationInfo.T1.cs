namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Represents an <see cref="IMessageHandler{TMessage}"/>-operation that is executed by a
    /// <see cref="IMicroProcessor" /> as part of a test.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by the message-handler.</typeparam>
    public sealed class MessageHandlerTestOperationInfo<TMessage> : MicroProcessorTestOperationInfo
    {
        /// <summary>
        /// Gets or sets the message that will be handled by the <see cref="IMessageHandler{TMessage}"/>.
        /// </summary>
        public TMessage Message
        {
            get;
            set;
        }
    }
}
