using System;
using System.Diagnostics.CodeAnalysis;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a sequence of messages, ready to be executed.
    /// </summary>
    public abstract class MessageSequence : IMessageSequence
    {
        #region [====== Nested Types ======]

        private sealed class EmptyCommandSequence : IMessageSequence
        {
            public void HandleWith(IMessageProcessor handler) { }

            public IMessageSequence Append(IMessageSequence sequence)
            {
                if (sequence == null)
                {
                    throw new ArgumentNullException("sequence");
                }
                return sequence;
            }

            public IMessageSequence Append<TMessage>(TMessage message) where TMessage : class
            {
                return new MessageSequenceNode<TMessage>(message);
            }
        }

        #endregion

        /// <inheritdoc />
        void IMessageSequence.HandleWith(IMessageProcessor processor)
        {
            HandleWith(processor);
        }

        /// <summary>
        /// Handles all messages of this sequence using the specified processor.
        /// </summary>
        /// <param name="processor">Processor that will be used to execute this sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/> is <c>null</c>.
        /// </exception>
        protected abstract void HandleWith(IMessageProcessor processor);

        /// <inheritdoc />
        public virtual IMessageSequence Append(IMessageSequence sequence)
        {
            return new MessageSequencePair(this, sequence);
        }

        /// <inheritdoc />
        public virtual IMessageSequence Append<TMessage>(TMessage message) where TMessage : class
        {
            return new MessageSequencePair(this, new MessageSequenceNode<TMessage>(message));
        }

        /// <summary>
        /// Represents a sequence with no elements.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104", Justification = "Exposed type is immutable.")]
        public static readonly IMessageSequence EmptySequence = new EmptyCommandSequence();
    }
}
