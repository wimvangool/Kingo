using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a read-only list of messages.
    /// </summary>
    public interface IMessageStream : IReadOnlyList<object>
    {        
        /// <summary>
        /// Appends the specified <paramref name="message"/> to this stream and associates it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        IMessageStream Append<TMessage>(TMessage message, Action<TMessage, IMicroProcessorContext> handler);

        /// <summary>
        /// Appends the specified <paramref name="message"/> to this stream and associates it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        IMessageStream Append<TMessage>(TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler);

        /// <summary>
        /// Appends the specified <paramref name="message"/> to this stream and associates it with the specified <paramref name="handler"/>.
        /// </summary>        
        /// <param name="message">Message to append.</param>
        /// <param name="handler">Optional handler to associate with the message.</param>
        /// <returns>A new stream containing the appended message.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        IMessageStream Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null);

        /// <summary>
        /// Appends the specified stream of messages to this stream.
        /// </summary>
        /// <param name="stream">The stream to append.</param>
        /// <returns>A new stream, containing the messages of both the current and the specified stream.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        IMessageStream AppendStream(IMessageStream stream);        

        /// <summary>
        /// Lets the specified <paramref name="handler"/> handle all messages of this stream and returns a stream of events.
        /// </summary>
        /// <param name="handler">A handler of messages.</param>      
        /// <returns>A stream of events.</returns>  
        Task HandleMessagesWithAsync(IMessageHandler handler);
    }
}
