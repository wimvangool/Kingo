using System;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Convenience class that provides a basic implementation of the <see cref="IMessage{T}" />-interface,
    /// in which change tracking and validation are supported.
    /// </summary>
    /// <typeparam name="TMessage">Type of the concrete message.</typeparam>   
    public abstract class Message<TMessage> : Message, IMessage<TMessage> where TMessage : Message<TMessage>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message" /> class.
        /// </summary>
        protected Message() { }        

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="message">The message to copy.</param>
        /// <param name="makeReadOnly">Indicates whether the new instance should be marked readonly.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        protected Message(Message message, bool makeReadOnly)
            : base(message, makeReadOnly) { }

        /// <inheritdoc />
        public abstract TMessage Copy(bool makeReadOnly);
    }
}
