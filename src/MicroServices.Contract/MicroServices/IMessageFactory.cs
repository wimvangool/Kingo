using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents a factory that can be used to create messages with a specific content and (optional) delivery date.
    /// </summary>
    public interface IMessageFactory : IMessageKindResolver, IMessageIdGenerator
    {
        /// <summary>
        /// Creates a new message with the specified <paramref name="content"/> and <paramref name="deliveryTime" />.
        /// </summary>
        /// <param name="content">Content of the message.</param>
        /// <param name="deliveryTime">Optional delivery date.</param>
        /// <returns>A new message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        IMessage CreateMessage(object content, DateTimeOffset? deliveryTime = null);

        /// <summary>
        /// Creates a new message with the specified <paramref name="content"/> and <paramref name="deliveryTime" />.
        /// </summary>
        /// <typeparam name="TContent"></typeparam>
        /// <param name="content">Content of the message.</param>
        /// <param name="deliveryTime">Optional delivery date.</param>
        /// <returns>A new message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        Message<TContent> CreateMessage<TContent>(TContent content, DateTimeOffset? deliveryTime = null);
    }
}
