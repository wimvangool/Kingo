using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension methods for instances that implement the <see cref="IMessageHandlerOperationTestResult" />
    /// </summary>
    public static class MessageHandlerOperationTestResultExtensions
    {
        /// <summary>
        /// Verifies that a test produces an empty message-stream.
        /// </summary>
        /// <param name="result">The result to verify.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The result is either not an message-stream, or the message-stream contains one or more events.
        /// </exception>
        public static void IsEmptyStream(this IMessageHandlerOperationTestResult result) =>
            NotNull(result).IsMessageStream(IsEmpty);

        private static void IsEmpty(MessageStream stream)
        {
            if (stream.Count == 0)
            {
                return;
            }
            throw NewStreamNotEmptyException(stream);
        }

        private static IMessageHandlerOperationTestResult NotNull(IMessageHandlerOperationTestResult result) =>
            result ?? throw new ArgumentNullException(nameof(result));

        private static Exception NewStreamNotEmptyException(MessageStream stream)
        {
            var messageFormat = ExceptionMessages.MessageHandlerOperationTestResult_StreamNotEmpty;
            var message = string.Format(messageFormat, stream.Count);
            return new TestFailedException(message);
        }
    }
}
