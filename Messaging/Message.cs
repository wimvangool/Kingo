using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;

namespace System.ComponentModel.Messaging
{
    /// <summary>
    /// Serves as a simple base-implementation of the <see cref="IMessage" /> interface.
    /// </summary>
    public abstract class Message : PropertyChangedBase, IMessage
    {        
        IMessage IMessage.Copy()
        {
            return Copy();
        }

        /// <summary>
        /// Creates and returns a copy of this message.
        /// </summary>
        /// <returns>A copy of this message.</returns>
        protected abstract Message Copy();

        /// <summary>
        /// Creates and returns a (materialized) copy of the specified collection.
        /// </summary>
        /// <typeparam name="TMessage">Type of the messages in the collection.</typeparam>
        /// <param name="messages">The collection to copy.</param>
        /// <returns>The copied collection, or <c>null</c> if <paramref name="messages" /> was <c>null</c>.</returns>
        public static IList<TMessage> Copy<TMessage>(IEnumerable<TMessage> messages) where TMessage : class, IMessage
        {
            return messages == null ? null : messages.Select(Copy).ToArray();
        }

        /// <summary>
        /// Creates and returns a copy of the specified <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to copy.</typeparam>
        /// <param name="message">The message to copy.</param>
        /// <returns>
        /// A copy of the specified <paramref name="message"/>, or <c>null</c> if <paramref name="message"/> was <c>null</c>.
        /// </returns>
        public static TMessage Copy<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            return message == null ? null : (TMessage) message.Copy();
        }
    }
}
