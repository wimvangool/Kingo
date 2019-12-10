using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a processor that can be used to handle specific messages or run specific tests
    /// as a means to setup another test.
    /// </summary>
    public interface IMicroProcessorOperationRunner
    {
        /// <summary>
        /// Creates and returns a <see cref="IMessageHandlerOperationRunner{TMessage}"/> that can be used to
        /// run the specified <paramref name="operation"/> with a specific message handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="operation">The operation to run.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <returns>
        /// A <see cref="IMessageHandlerOperationRunner{TMessage}"/> that
        /// can be used to run the specified <paramref name="operation"/>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operation"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        IMessageHandlerOperationRunner<TMessage> Run<TMessage>(MessageHandlerOperation<TMessage> operation, MicroProcessorOperationTestContext context);

        /// <summary>
        /// Runs the specified <paramref name="operation"/> and stores its result into the specified <paramref name="context" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the <paramref name="operation"/>.</typeparam>
        /// <param name="operation">The operation to run.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operation"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        Task RunAsync<TMessage>(IMessageHandlerOperationTest<TMessage> operation, MicroProcessorOperationTestContext context);
    }
}
