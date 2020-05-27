using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a component that can be used to generate
    /// the <see cref="IMessage.MessageId" /> for each message.
    /// </summary>
    public interface IMessageIdGenerator
    {
        /// <summary>
        /// Generates a <see cref="IMessage.MessageId" /> for a message that has the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">Content of the message for which the message-id is to be generated.</param>
        /// <returns>A message-id.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        string GenerateMessageId(object content);
    }
}
