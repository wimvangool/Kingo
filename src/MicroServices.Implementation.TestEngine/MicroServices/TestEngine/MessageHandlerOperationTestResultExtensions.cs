using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// Contains extension methods for instances that implement the <see cref="IMessageHandlerOperationTestResult" />
    /// </summary>
    public static class MessageHandlerOperationTestResultExtensions
    {
        /// <summary>
        /// Verifies that no messages were produced.
        /// </summary>
        /// <param name="result">The result to verify.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The result is either not an message-stream, or the message-stream contains one or more messages.
        /// </exception>
        public static void IsEmptyStream(this IMessageHandlerOperationTestResult result) =>
            result.IsMessageStream(0);

        /// <summary>
        /// Verifies that a specific amount of messages were produced.
        /// </summary>
        /// <param name="result">The result to verify.</param>
        /// <param name="length">The expected length of the stream.</param>
        /// <param name="assertion">
        /// Optional delegate to verify the details of all the messages.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is negative.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The result is either not an message-stream,
        /// or the length of the stream does not match the specified <paramref name="length"/>.
        /// </exception>
        public static void IsMessageStream(this IMessageHandlerOperationTestResult result, int length, Action<MessageStream> assertion = null) =>
            NotNull(result).IsMessageStream(length, length, assertion);

        private static IMessageHandlerOperationTestResult NotNull(IMessageHandlerOperationTestResult result) =>
            result ?? throw new ArgumentNullException(nameof(result));
    }
}
