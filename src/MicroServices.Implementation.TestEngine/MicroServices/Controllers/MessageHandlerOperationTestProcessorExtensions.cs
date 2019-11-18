using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extensions methods for instances that implement the <see cref="IMessageHandlerOperationTestProcessor" /> interface.
    /// </summary>
    public static class MessageHandlerOperationTestProcessorExtensions
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
        /// <paramref name="processor"/>, <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IMessageHandlerOperationTestProcessor processor, TMessage message, MicroProcessorOperationTestContext context, Action<TMessage, IMessageHandlerOperationContext> handler) =>
            NotNull(processor).ExecuteCommandAsync(MessageHandlerDecorator<TMessage>.Decorate(handler), message, context);

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="processor">The processor to handle the message with.</param>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="handler">Optional handler to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="processor"/>, <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IMessageHandlerOperationTestProcessor processor, TMessage message, MicroProcessorOperationTestContext context, Func<TMessage, IMessageHandlerOperationContext, Task> handler) =>
            NotNull(processor).ExecuteCommandAsync(MessageHandlerDecorator<TMessage>.Decorate(handler), message, context);

        private static IMessageHandlerOperationTestProcessor NotNull(IMessageHandlerOperationTestProcessor processor) =>
            processor ?? throw new ArgumentNullException(nameof(processor));
    }
}
