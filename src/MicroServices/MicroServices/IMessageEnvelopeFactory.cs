using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a factory for <see cref="IMessageEnvelope"/> objects.
    /// </summary>
    public interface IMessageEnvelopeFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="MessageEnvelope" /> with the specified <paramref name="message" />.
        /// </summary>
        /// <param name="message">Content of the message.</param>
        /// <returns>A new <see cref="MessageEnvelope"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        MessageEnvelope Wrap(object message);

        /// <summary>
        /// Creates and returns a new <see cref="MessageEnvelope{TMessage}" /> with the specified <paramref name="message" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message of the message.</typeparam>
        /// <param name="message">Content of the message.</param>
        /// <returns>A new <see cref="MessageEnvelope"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        MessageEnvelope<TMessage> Wrap<TMessage>(TMessage message);
    }
}
