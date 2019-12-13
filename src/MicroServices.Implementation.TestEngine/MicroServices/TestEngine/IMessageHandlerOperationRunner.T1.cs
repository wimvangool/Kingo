using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a component that can be used to run a specific message
    /// using a specific <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageHandlerOperationRunner<TMessage>
    {
        #region [====== ExecuteCommandAsync ======]

        /// <summary>
        /// Executes a command using the specified <typeparamref name="TMessageHandler"/>.
        /// </summary>
        /// <typeparam name="TMessageHandler">Type of the message handler that will handle the specified <paramref name="message"/>.</typeparam>
        /// <param name="message">The command to execute.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task ExecuteCommandAsync<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage>;

        /// <summary>
        /// Executes a command using the specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">Message handler that will handle the specified <paramref name="message"/>.</param>
        /// <param name="message">The command to execute.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task ExecuteCommandAsync(IMessageHandler<TMessage> messageHandler, TMessage message);

        #endregion

        #region [====== HandleEventAsync ======]

        /// <summary>
        /// Handles an event using the specified <typeparamref name="TMessageHandler"/>.
        /// </summary>
        /// <typeparam name="TMessageHandler">Type of the message handler that will handle the specified <paramref name="message"/>.</typeparam>
        /// <param name="message">The event to handle.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task HandleEventAsync<TMessageHandler>(TMessage message) where TMessageHandler : class, IMessageHandler<TMessage>;

        /// <summary>
        /// Handles an event using the specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <param name="messageHandler">Message handler that will handle the specified <paramref name="message"/>.</param>
        /// <param name="message">The event to handle.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        Task HandleEventAsync(IMessageHandler<TMessage> messageHandler, TMessage message);

        #endregion
    }
}
