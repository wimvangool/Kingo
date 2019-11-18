using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents the result of a <see cref="IMessageHandlerOperationTest{TMessage,TOutputStream}"/>,
    /// where the result is either an exception or an empty message-stream.
    /// </summary>
    public interface IMessageHandlerOperationTestResult : IMicroProcessorOperationTestResult
    {
        /// <summary>
        /// Verifies that no events were published.
        /// </summary>
        /// <param name="assertion">
        /// Optional delegate to verify the details of all the messages.
        /// </param>
        void IsMessageStream(Action<MessageStream> assertion = null);
    }
}
