using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a message handler operation for a specific type of message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this operation.</typeparam>
    public interface IMessageHandlerOperation<TMessage> : IMessageHandlerOperation
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
    }
}
