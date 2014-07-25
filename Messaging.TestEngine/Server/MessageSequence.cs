using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.Messaging.Server
{
    /// <summary>
    /// Represents a sequence of messages, ready to be executed.
    /// </summary>
    public abstract class MessageSequence : IMessageSequence
    {
        #region [====== Nested Types ======]

        private sealed class EmptyCommandSequence : IMessageSequence
        {
            public void ProcessWith(IMessageProcessor handler) { }

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
        public abstract void ProcessWith(IMessageProcessor processor);

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
