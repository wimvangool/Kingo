using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a sequence of messages, ready to be executed.
    /// </summary>
    public abstract class MessageSequence : IMessageSequence
    {        
        /// <inheritdoc />
        public virtual void ProcessWith(IMessageProcessor processor)
        {
            ProcessWithAsync(processor).Wait();
        }

        /// <inheritdoc />
        public Task ProcessWithAsync(IMessageProcessor processor)
        {
            return ProcessWithAsync(processor, CancellationToken.None);
        }

        /// <inheritdoc />
        public abstract Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token);

        /// <inheritdoc />
        public virtual IMessageSequence Append(IMessageSequence sequence)
        {
            return new MessageSequencePair(this, sequence);
        }

        /// <inheritdoc />
        public IMessageSequence Append<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            return new MessageSequencePair(this, new MessageToHandle<TMessage>(message));
        }

        /// <inheritdoc />
        public IMessageSequence Append<TMessage>(TMessage message, Action<TMessage> handler) where TMessage : class, IMessage
        {
            return Append(message, new MessageHandlerDelegate<TMessage>(handler));
        }

        /// <inheritdoc />
        public IMessageSequence Append<TMessage>(TMessage message, Func<TMessage, Task> handler) where TMessage : class, IMessage
        {
            return Append(message, new MessageHandlerDelegate<TMessage>(handler));
        }

        /// <inheritdoc />
        public virtual IMessageSequence Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class, IMessage
        {
            return new MessageSequencePair(this, new MessageToHandle<TMessage>(message, handler));
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
