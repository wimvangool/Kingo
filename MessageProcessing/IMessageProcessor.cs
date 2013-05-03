using System;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented by a class, represents a handler of any command.
    /// </summary>
    public interface IMessageProcessor
    {                
        /// <summary>
        /// Handles the specified message by invoking all registered external message handlers.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        void Handle<TMessage>(TMessage message) where TMessage : class;

        /// <summary>
        /// Handles the specified message by invoking the specified handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="handler">Handler that will be used to handle the message.</param>
        void Handle<TMessage>(TMessage message, IExternalMessageHandler<TMessage> handler) where TMessage : class;

        /// <summary>
        /// Handles the specified message by invoking the specified delegate.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message.</typeparam>
        /// <param name="message">Message to handle.</param>
        /// <param name="action">Delegate that will be used to handle the message.</param>
        void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class;
    }
}
