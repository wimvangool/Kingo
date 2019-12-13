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
        /// Verifies that one specific message was produced.
        /// </summary>
        /// <typeparam name="TMessage">Type of the content of the message.</typeparam>
        /// <param name="result">The result to verify.</param>
        /// <param name="assertion">
        /// Optional delegate to verify the details of the message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The result is not an message-stream
        /// - or -
        /// The stream is empty
        /// - or -
        /// More than once message was produced
        /// - or -
        /// The message is not of the specified type <typeparamref name="TMessage" />.
        /// </exception>
        public static void Is<TMessage>(this IMessageHandlerOperationTestResult result, Action<TMessage> assertion = null)
        {
            if (assertion == null)
            {
                result.Is<TMessage>(message => { });
                return;
            }
            result.IsMessageStream(1, stream =>
            {
                stream.Assert(assertion);
            });
        }

        /// <summary>
        /// Verifies that one specific message was produced.
        /// </summary>
        /// <typeparam name="TMessage">Type of the content of the message.</typeparam>
        /// <param name="result">The result to verify.</param>
        /// <param name="assertion">
        /// Optional delegate to verify the details of the message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="TestFailedException">
        /// The result is not an message-stream
        /// - or -
        /// The stream is empty
        /// - or -
        /// More than once message was produced
        /// - or -
        /// The message is not of the specified type <typeparamref name="TMessage" />.
        /// </exception>
        public static void IsMessage<TMessage>(this IMessageHandlerOperationTestResult result, Action<MessageToDispatch<TMessage>> assertion = null)
        {
            if (assertion == null)
            {
                result.IsMessage<TMessage>(message => { });
                return;
            }
            result.IsMessageStream(1, stream =>
            {
                stream.AssertMessage(assertion);
            });
        }

        /// <summary>
        /// Verifies that a specific amount of messages were produced.
        /// </summary>
        /// <param name="result">The result to verify.</param>
        /// <param name="length">The expected length of the stream.</param>
        /// <param name="assertion">
        /// Optional delegate to verify the details of all the messages.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="result"/> is <c>null</c>.
        /// </exception>
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
