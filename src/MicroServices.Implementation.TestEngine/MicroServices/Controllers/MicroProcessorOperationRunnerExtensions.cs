using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IMicroProcessorOperationRunner" />.
    /// </summary>
    public static class MicroProcessorOperationRunnerExtensions
    {
        /// <summary>
        /// Creates and returns a <see cref="IMessageHandlerOperationRunner{TMessage}"/> that can be used to
        /// run a <see cref="MessageHandlerOperation{TMessage}"/> with a specific message handler.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
        /// <param name="runner">The runner that will run the operation.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <returns>
        /// A <see cref="IMessageHandlerOperationRunner{TMessage}"/> that
        /// can be used to run the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runner"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static IMessageHandlerOperationRunner<TMessage> RunOperation<TMessage>(this IMicroProcessorOperationRunner runner, MicroProcessorOperationTestContext context) =>
            NotNull(runner).Run(new MessageHandlerOperation<TMessage>(), context);

        private static IMicroProcessorOperationRunner NotNull(IMicroProcessorOperationRunner runner) =>
            runner ?? throw new ArgumentNullException(nameof(runner));
    }
}
