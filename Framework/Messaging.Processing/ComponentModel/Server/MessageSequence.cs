using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a sequence of messages, ready to be executed.
    /// </summary>
    public abstract class MessageSequence : IMessageSequence
    {
        #region [====== Nested Types ======]

        private sealed class EmptyMessageSequence : IMessageSequence
        {
            public void ProcessWith(IMessageProcessor handler) { }

            public Task ProcessWithAsync(IMessageProcessor processor)
            {
                return AsyncMethod.Void;
            }

            public IMessageSequence Append(IMessageSequence sequence)
            {
                if (sequence == null)
                {
                    throw new ArgumentNullException("sequence");
                }
                return sequence;
            }

            public IMessageSequence Append<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
            {
                return new MessageSequenceNode<TMessage>(message);
            }            
        }

        #endregion

        /// <inheritdoc />
        public virtual void ProcessWith(IMessageProcessor processor)
        {
            ProcessWithAsync(processor).Wait();
        }

        /// <inheritdoc />
        public abstract Task ProcessWithAsync(IMessageProcessor processor);

        /// <inheritdoc />
        public virtual IMessageSequence Append(IMessageSequence sequence)
        {
            return new MessageSequencePair(this, sequence);
        }

        /// <inheritdoc />
        public virtual IMessageSequence Append<TMessage>(TMessage message) where TMessage : class, IMessage<TMessage>
        {
            return new MessageSequencePair(this, new MessageSequenceNode<TMessage>(message));
        }

        /// <summary>
        /// Represents a sequence with no elements.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104", Justification = "Exposed type is immutable.")]
        public static readonly IMessageSequence EmptySequence = new EmptyMessageSequence();

        /// <summary>
        /// Creates and returns a concatened sequence of messages.
        /// </summary>
        /// <param name="messages">The messages to concatenate.</param>
        /// <returns>A concatened sequence of messages.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messages"/> is <c>null</c>.
        /// </exception>
        public static IMessageSequence Concatenate(IEnumerable<IMessageSequence> messages)
        {
            return new MessageSequenceConcatenation(messages);
        }        
    }
}
