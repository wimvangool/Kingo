using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extensions methods for instances that implement the <see cref="IHandleMessageOperationTestProcessor" /> interface.
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
        /// <paramref name="processor"/>, <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static Task HandleAsync<TMessage>(this IHandleMessageOperationTestProcessor processor, TMessage message, MicroProcessorOperationTestContext context, Action<TMessage, MessageHandlerOperationContext> handler) =>
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
        public static Task HandleAsync<TMessage>(this IHandleMessageOperationTestProcessor processor, TMessage message, MicroProcessorOperationTestContext context, Func<TMessage, MessageHandlerOperationContext, Task> handler) =>
            NotNull(processor).ExecuteCommandAsync(MessageHandlerDecorator<TMessage>.Decorate(handler), message, context);

        private static IHandleMessageOperationTestProcessor NotNull(IHandleMessageOperationTestProcessor processor) =>
            processor ?? throw new ArgumentNullException(nameof(processor));
    }
}
