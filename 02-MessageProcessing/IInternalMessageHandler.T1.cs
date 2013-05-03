
namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, handles messages from the <see cref="DomainEventBus "/> of a specific type.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled.</typeparam>
    public interface IInternalMessageHandler<in TMessage> where TMessage : class
    {
        /// <summary>
        /// Handles the messages.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        void Handle(TMessage message);
    }
}
