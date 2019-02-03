using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension-methods  for instances that implement the <see cref="IMessageProcessor{TMessage}"/> interface.
    /// </summary>
    public static class MessageProcessorExtensions
    {
        /// <summary>
        /// Processes the specified <paramref name="message" />.
        /// </summary>
        /// <param name="processor">The processor that will process the message.</param>
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">Optional handler to handle the message inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IMessageProcessor<TMessage> processor, TMessage message, Action<TMessage, MessageHandlerContext> handler) =>
            processor.HandleAsync(message, MessageHandlerDecorator<TMessage>.Decorate(handler));

        /// <summary>
        /// Processes the specified <paramref name="message" />.
        /// </summary>
        /// <param name="processor">The processor that will process the message.</param>
        /// <param name="message">The message to handle.</param>
        /// <param name="handler">Optional handler to handle the message inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IMessageProcessor<TMessage> processor, TMessage message, Func<TMessage, MessageHandlerContext, Task> handler) =>
            processor.HandleAsync(message, MessageHandlerDecorator<TMessage>.Decorate(handler));
    }
}
