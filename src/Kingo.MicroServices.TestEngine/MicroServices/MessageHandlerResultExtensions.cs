using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for instances that implement the <see cref="IMessageHandlerResult" />
    /// </summary>
    public static class MessageHandlerResultExtensions
    {
        /// <summary>
        /// Verifies that a test produces an empty event-stream.
        /// </summary>
        /// <param name="result">The result to verify.</param>
        /// <exception cref="TestFailedException">
        /// The result is either not an event-stream, or the event-stream contains one or more events.
        /// </exception>
        public static void IsEmptyStream(this IMessageHandlerResult result) =>
            result.IsEventStream(IsEmpty);

        private static void IsEmpty(EventStream stream)
        {
            if (stream.Count == 0)
            {
                return;
            }
            throw NewStreamNotEmptyException(stream);
        }

        private static Exception NewStreamNotEmptyException(EventStream stream)
        {
            var messageFormat = ExceptionMessages.MessageHandlerResult_StreamNotEmpty;
            var message = string.Format(messageFormat, stream.Count);
            return new TestFailedException(message);
        }
    }
}
