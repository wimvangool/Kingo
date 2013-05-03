
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, handles messages from an external message-bus of a specific type.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled.</typeparam>
    public interface IExternalMessageHandler<in TMessage> where TMessage : class
    {
        /// <summary>
        /// Handles the messages.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        void Handle(TMessage message);
    }
}
