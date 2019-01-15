using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a processor that can be used to handle specific messages or run specific tests
    /// as a means to setup another test.
    /// </summary>
    public interface IHandleMessageTestProcessor
    {
        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="handler">Optional handler to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        Task HandleAsync<TMessage>(TMessage message, MicroProcessorTestContext context, Func<TMessage, MessageHandlerContext, Task> handler);

        /// <summary>
        /// Handles the specified message.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="message">The message to handle.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="handler">Optional handler to handle the message with inside the processor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> <paramref name="context"/> is <c>null</c>.
        /// </exception>
        Task HandleAsync<TMessage>(TMessage message, MicroProcessorTestContext context, IMessageHandler<TMessage> handler = null);

        /// <summary>
        /// Runs the specified <paramref name="test"/> and stores it's result into the specified <paramref name="context" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the <paramref name="test"/>.</typeparam>
        /// <typeparam name="TEventStream">Type of the event-stream that will be produced by the <paramref name="test"/>.</typeparam>
        /// <param name="test">The test to run.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        Task HandleAsync<TMessage, TEventStream>(IHandleMessageTest<TMessage, TEventStream> test, MicroProcessorTestContext context) where TEventStream : EventStream;
    }
}
