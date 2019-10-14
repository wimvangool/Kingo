using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a factory for <see cref="IMessage"/> objects.
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="Message" /> with the specified <paramref name="content" />.
        /// </summary>
        /// <param name="content">Content of the message.</param>
        /// <returns>A new <see cref="Message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        Message CreateMessage(object content);

        /// <summary>
        /// Creates and returns a new <see cref="Message{TMessage}" /> with the specified <paramref name="content" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the content of the message.</typeparam>
        /// <param name="content">Content of the message.</param>
        /// <returns>A new <see cref="Message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        Message<TMessage> CreateMessage<TMessage>(TMessage content);
    }
}
