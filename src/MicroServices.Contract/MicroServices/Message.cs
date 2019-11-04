using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all messages that are able to generate their own identifiers.
    /// </summary>
    [Serializable]
    public abstract class Message : DataContract, IMessage
    {
        string IMessage.GenerateMessageId() =>
            GenerateMessageId();

        /// <summary>
        /// Generates a new message-identifier for this message.
        /// </summary>
        /// <returns>A new message-identifier for this message.</returns>
        protected virtual string GenerateMessageId() =>
            NewMessageId();

        /// <summary>
        /// Generates and returns a new, random message-id.
        /// </summary>
        /// <returns>A new message-id.</returns>
        public static string NewMessageId() =>
            Guid.NewGuid().ToString();
    }
}
