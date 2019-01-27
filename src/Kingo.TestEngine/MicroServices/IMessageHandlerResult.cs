using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of a <see cref="IMessageHandlerTest{TMessage,TEventStream}"/>,
    /// where the result is either an exception or an empty event-stream.
    /// </summary>
    public interface IMessageHandlerResult : IMicroProcessorTestResult
    {
        /// <summary>
        /// Verifies that no events were published.
        /// </summary>
        /// <param name="assertion">
        /// Optional delegate to verify the details of all the published events.
        /// </param>
        void IsEventStream(Action<EventStream> assertion = null);
    }
}
