using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a message handler operation for a specific type of message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this operation.</typeparam>
    public interface IMessageHandlerOperation<TMessage>
    {
        /// <summary>
        /// Returns the message that was handled by the message handler.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>
        /// <returns>The message that was handled by the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// This operation has not been executed yet.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        MessageEnvelope<TMessage> GetInputMessage(MicroProcessorOperationTestContext context);

        /// <summary>
        /// Returns the stream of commands and events that were produced by the operation.
        /// </summary>
        /// <param name="context">The context in which the test is running.</param>
        /// <returns>The stream of commands and events that were produced by the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// This operation has not been executed yet.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        MessageStream GetOutputStream(MicroProcessorOperationTestContext context);
    }
}
