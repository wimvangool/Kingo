using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extensions methods for instances that implement the <see cref="IMessageHandlerTestProcessor" /> interface.
    /// </summary>
    public static class MessageHandlerTestProcessorExtensions
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="processor">The processor to handle the message with.</param>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="handler">Optional handler to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IMessageHandlerTestProcessor processor, TMessage message, MicroProcessorTestContext context, Action<TMessage, MessageHandlerOperationContext> handler) =>
            processor.HandleAsync(message, context, MessageHandlerDecorator<TMessage>.Decorate(handler));

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="processor">The processor to handle the message with.</param>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="handler">Optional handler to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IMessageHandlerTestProcessor processor, TMessage message, MicroProcessorTestContext context, Func<TMessage, MessageHandlerOperationContext, Task> handler) =>
            processor.HandleAsync(message, context, MessageHandlerDecorator<TMessage>.Decorate(handler));
    }
}
