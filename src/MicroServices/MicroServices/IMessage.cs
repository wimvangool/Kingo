namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a message that can be sent, received, processed and persisted.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Generates a new message-identifier for this message.
        /// </summary>
        /// <returns>A new message-identifier for this message.</returns>
        string GenerateMessageId();
    }
}
