using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents a message handler operation for a specific type of message.
    /// </summary>
    public interface IMessageHandlerOperation
    {
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
