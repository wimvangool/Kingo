using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a sequence containing just a single message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message.</typeparam>
    public class MessageSequenceNode<TMessage> : MessageSequence
        where TMessage : class
    {
        private readonly TMessage _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSequenceNode{T}" /> class.
        /// </summary>
        /// <param name="message">The message of this sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public MessageSequenceNode(TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;
        }

        /// <summary>
        /// The message of this sequence.
        /// </summary>
        protected TMessage Command
        {
            get { return _message; }
        }

        /// <inheritdoc />
        public override void HandleWith(IMessageProcessor handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            handler.Handle(_message);
        }
    }
}
