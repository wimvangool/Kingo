using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Contains extension methods for instances that implement the <see cref="IHandleMessageResult" />
    /// </summary>
    public static class HandleMessageResultExtensions
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
        public static void IsEmptyStream(this IHandleMessageResult result) =>
            NotNull(result).IsMessageStream(IsEmpty);

        private static void IsEmpty(MessageStream stream)
        {
            if (stream.Count == 0)
            {
                return;
            }
            throw NewStreamNotEmptyException(stream);
        }

        private static IHandleMessageResult NotNull(IHandleMessageResult result) =>
            result ?? throw new ArgumentNullException(nameof(result));

        private static Exception NewStreamNotEmptyException(MessageStream stream)
        {
            var messageFormat = ExceptionMessages.MessageHandlerResult_StreamNotEmpty;
            var message = string.Format(messageFormat, stream.Count);
            return new TestFailedException(message);
        }
    }
}
