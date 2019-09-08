using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension-methods  for instances that implement the <see cref="IMessageProcessor{TMessage}"/> interface.
    /// </summary>
    public static class MessageProcessorExtensions
    {
        #region [====== ExecuteCommandAsync ======]

        /// <summary>
        /// Executes a command with a specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <param name="processor">The processor that will process the message.</param>
        /// <param name="messageHandler">Delegate that will execute the command.</param>
        /// <param name="message">The command to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteCommandAsync<TMessage>(this IMessageProcessor<TMessage> processor, Action<TMessage, MessageHandlerOperationContext> messageHandler, TMessage message) =>
            processor.ExecuteCommandAsync(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Processes the specified <paramref name="message" />.
        /// </summary>
        /// <param name="processor">The processor that will process the message.</param>
        /// <param name="messageHandler">Delegate that will execute the command.</param>
        /// <param name="message">The command to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static Task ExecuteCommandAsync<TMessage>(this IMessageProcessor<TMessage> processor, Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, TMessage message) =>
            processor.ExecuteCommandAsync(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        #endregion

        #region [====== HandleEventAsync ======]

        /// <summary>
        /// Processes the specified <paramref name="message"/> with the specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <param name="processor">The processor that will process the message.</param>
        /// <param name="messageHandler">Delegate that will handle the event.</param>
        /// <param name="message">The event to handle.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static Task HandleEventAsync<TMessage>(this IMessageProcessor<TMessage> processor, Action<TMessage, MessageHandlerOperationContext> messageHandler, TMessage message) =>
            processor.HandleEventAsync(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        /// <summary>
        /// Processes the specified <paramref name="message"/> with the specified <paramref name="messageHandler"/>.
        /// </summary>
        /// <param name="processor">The processor that will process the message.</param>
        /// <param name="messageHandler">Delegate that will handle the event.</param>
        /// <param name="message">The event to handle.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> or <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static Task HandleEventAsync<TMessage>(this IMessageProcessor<TMessage> processor, Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, TMessage message) =>
            processor.HandleEventAsync(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), message);

        #endregion
    }
}
