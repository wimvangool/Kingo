using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents the result of a <see cref="IMessageHandlerOperationTest{TMessage,TOutputStream}"/>.
    /// </summary>
    /// <typeparam name="TOutputStream">Type of the event-stream produced by the test.</typeparam>
    public interface IMessageHandlerOperationTestResult<in TOutputStream> : IMicroProcessorOperationTestResult
        where TOutputStream : MessageStream
    {
        /// <summary>
        /// Asserts that the test produced a specific set of messages.
        /// </summary>        
        /// <param name="assertion">
        /// Delegate to verify the details of all the messages. When verified, the delegate also creates and returns
        /// a message stream of type <typeparamref name="TOutputStream" /> that can be used by other tests
        /// to access the contents of each message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertion"/> is <c>null</c>.
        /// </exception>        
        void IsMessageStream(Func<MessageStream, TOutputStream> assertion);
    }
}
