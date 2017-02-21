using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a handler of different message types.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">The message to handle.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        void Handle<TMessage>(TMessage message) where TMessage : class, IMessage;
    }
}
