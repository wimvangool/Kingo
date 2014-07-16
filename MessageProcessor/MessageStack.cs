using System;
using System.Threading;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents the stack of messages that is being handled by the processor.
    /// </summary>
    public sealed class MessageStack
    {
        /// <summary>
        /// The actual message instance.
        /// </summary>
        public readonly object Instance;

        /// <summary>
        /// The <see cref="CancellationToken" /> that was specified when handling this message.
        /// </summary>
        public readonly CancellationToken? CancellationToken;

        /// <summary>
        /// The message of the outer scope.
        /// </summary>
        public readonly MessageStack PreviousMessage;
        
        internal MessageStack(object instance, CancellationToken? token)
        {
            Instance = instance;
            CancellationToken = token;
        }

        private MessageStack(object instance, CancellationToken? token, MessageStack previousMessage)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            Instance = instance;
            CancellationToken = token;
            PreviousMessage = previousMessage;
        }

        internal MessageStack NextMessage(object instance, CancellationToken? token)
        {
            return new MessageStack(instance, token, this);
        }

        /// <summary>
        /// Indicates whether or not this message is of the specified type.
        /// </summary>
        /// <typeparam name="TMessage">Type to check.</typeparam>
        /// <returns><c>true</c> if this message is of the specified type; otherwise <c>false</c>.</returns>
        public bool IsA<TMessage>()
        {
            return IsA(typeof(TMessage));
        }

        /// <summary>
        /// Indicates whether or not this message is of the specified type.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><c>true</c> if this message is of the specified type; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool IsA(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.IsInstanceOfType(Instance);
        }

        /// <summary>
        /// Traverses up the stack looking for a token that indicates that a cancellation is requested and
        /// if found, throws an <see cref="OperationCanceledException" />.
        /// </summary>
        public void ThrowIfCancellationRequested()
        {
            MessageStack stack = this;

            do
            {
                stack.CancellationToken.ThrowIfCancellationRequested();
            } while ((stack = stack.PreviousMessage) != null);
        }
    }
}
