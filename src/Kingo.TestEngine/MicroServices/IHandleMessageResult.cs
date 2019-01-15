using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of a <see cref="IHandleMessageTest{TMessage,TEventStream}"/>,
    /// where the result is either an exception or an empty event-stream.
    /// </summary>
    public interface IHandleMessageResult : IMicroProcessorTestResult
    {
        /// <summary>
        /// Verifies that no events were published.
        /// </summary>
        void IsExpectedEventStream(Action<EventStream> assertion = null);


    }
}
