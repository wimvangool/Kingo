using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// Represents a message that is being handled by the processor.
    /// </summary>
    public sealed class Message
    {
        /// <summary>
        /// The actual message instance.
        /// </summary>
        public readonly object Instance;

        /// <summary>
        /// The message of the outer scope.
        /// </summary>
        public readonly Message PreviousMessage;
        
        internal Message(object instance)
        {
            Instance = instance;            
        }

        private Message(object instance, Message previousMessage)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            Instance = instance;
            PreviousMessage = previousMessage;
        }

        internal Message NextMessage(object instance)
        {
            return new Message(instance, this);
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
    }
}
