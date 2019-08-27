using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a processor that can be used to handle specific messages or run specific tests
    /// as a means to setup another test.
    /// </summary>
    public interface IHandleMessageOperationTestProcessor
    {
        /// <summary>
        /// Executes a command with a specific message handler.
        /// </summary>
        /// <typeparam name="TCommand">Type of the command to handle.</typeparam>
        /// <param name="messageHandler">The <see cref="IMessageHandler{TMessage}"/> that is to handle the specified command.</param>
        /// <param name="message">The command to execute.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/>, <paramref name="message"/>, or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        Task ExecuteCommandAsync<TCommand>(IMessageHandler<TCommand> messageHandler, TCommand message, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Handles an event with a specific event handler.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to handle.</typeparam>
        /// <param name="messageHandler">The <see cref="IMessageHandler{TMessage}"/> that is to handle the specified event.</param>
        /// <param name="message">The event to handle.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/>, <paramref name="message"/>, or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        Task HandleEventAsync<TEvent>(IMessageHandler<TEvent> messageHandler, TEvent message, MicroProcessorOperationTestContext context);

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
        Task RunAsync<TMessage, TEventStream>(IHandleMessageTest<TMessage, TEventStream> test, MicroProcessorOperationTestContext context) where TEventStream : EventStream;
    }
}
