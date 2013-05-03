using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, represents a sequence of messages, ready to be executed.
    /// </summary>
    public interface IMessageSequence
    {
        /// <summary>
        /// Handles all messages.
        /// </summary>
        /// <param name="handler">Handler that will be used to execute this sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        void HandleWith(IMessageProcessor handler);

        /// <summary>
        /// Appends the specified sequence to the current sequence and returns the resulting sequence.
        /// </summary>
        /// <param name="sequence">The sequence of messages to append.</param>
        /// <returns>The new, concatenated sequence of messages.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is <c>null</c>.
        /// </exception>
        IMessageSequence Append(IMessageSequence sequence);

        /// <summary>
        /// Appends the specified message to the current sequence and returns the resulting sequence.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to append.</typeparam>
        /// <param name="message">The message to append.</param>
        /// <returns>The new, concatenated sequence of messages.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        IMessageSequence Append<TMessage>(TMessage message) where TMessage : class;
    }
}
