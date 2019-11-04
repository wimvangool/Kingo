namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents (the envelope of) a message, carrying its payload and metadata.
    /// </summary>
    public interface IMessageEnvelope
    {
        /// <summary>
        /// Returns the unique identifier of this message.
        /// </summary>
        string MessageId
        {
            get;
        }

        /// <summary>
        /// If specified, returns the message-id of the message this message is correlated with
        /// (which is typically the message that triggered this message to be dispatched or processed).
        /// </summary>
        string CorrelationId
        {
            get;
        }

        /// <summary>
        /// Returns the contents/payload of the message.
        /// </summary>
        object Content
        {
            get;
        }
    }
}
