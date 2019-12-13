using System;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the result of a <see cref="IMessageHandlerOperationTest{TMessage}"/>,
    /// where the result is either an exception or an empty message-stream.
    /// </summary>
    public interface IMessageHandlerOperationTestResult : IMicroProcessorOperationTestResult
    {
        /// <summary>
        /// Verifies that a number of messages were produced.
        /// </summary>
        /// <param name="minLength">The expected minimum length of the stream.</param>
        /// <param name="maxLength">The expected maximum length of the stream.</param>
        /// <param name="assertion">
        /// Optional delegate to verify the details of all the messages.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minLength"/> is negative or is greater than the specified <paramref name="maxLength" />.
        /// </exception>
        ///  <exception cref="TestFailedException">
        /// The result is either not an message-stream,
        /// or the length of the stream is not in range of the specified <paramref name="minLength"/>
        /// and <paramref name="maxLength"/>.
        /// </exception>
        void IsMessageStream(int minLength, int maxLength, Action<MessageStream> assertion = null);
    }
}
