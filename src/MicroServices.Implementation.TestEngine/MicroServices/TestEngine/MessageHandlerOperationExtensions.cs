using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IMessageHandlerOperation{TMessage}"/>.
    /// </summary>
    public static class MessageHandlerOperationExtensions
    {
        /// <summary>
        /// Returns the message(-content) that was handled by the message handler.
        /// </summary>
        /// <param name="operation">The operation that was executed.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <returns>The message that was handled by the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// This operation has not been executed yet.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operation"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static TMessage GetInput<TMessage>(this IMessageHandlerOperation<TMessage> operation, MicroProcessorOperationTestContext context) =>
            NotNull(operation).GetInputMessage(context).Content;

        /// <summary>
        /// Returns the message(-content) of type <typeparamref name="TMessage" /> that is at the relative
        /// <paramref name="index"/> of the output stream of this operation.
        /// </summary>
        /// <param name="operation">The operation that was executed.</param>
        /// <param name="context">The context in which the test is running.</param>
        /// <param name="index">
        /// The relative index of the message in the stream. This index applies to the list of all messages
        /// of type <typeparamref name="TMessage"/> in the stream.
        /// </param>
        /// <returns>The message that was handled by the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// This operation has not been executed yet.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="operation"/> or <paramref name="context"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The output-stream does not contain a message of type <typeparamref name="TMessage"/> at the specified <paramref name="index"/>.
        /// </exception>
        public static TMessage GetOutput<TMessage>(this IMessageHandlerOperation operation, MicroProcessorOperationTestContext context, int index = 0) =>
            NotNull(operation).GetOutputStream(context).GetMessage<TMessage>(index).Content;

        private static TOperation NotNull<TOperation>(TOperation operation) where TOperation : class =>
            operation ?? throw new ArgumentNullException(nameof(operation));
    }
}
