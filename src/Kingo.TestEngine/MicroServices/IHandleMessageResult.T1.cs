using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a <see cref="IHandleMessageTest{TMessage,TEventStream}"/>.
    /// </summary>
    /// <typeparam name="TEventStream">Type of the event-stream produced by the test.</typeparam>
    public interface IHandleMessageResult<in TEventStream> : IMicroProcessorTestResult
        where TEventStream : EventStream
    {
        /// <summary>
        /// Asserts that the test produced a specific set of events.
        /// </summary>        
        /// <param name="assertion">
        /// Delegate to verify the details of all the published events and to create and return
        /// an event stream of type <typeparamref name="TEventStream" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertion"/> is <c>null</c>.
        /// </exception>        
        void IsExpectedEventStream(Func<EventStream, TEventStream> assertion);
    }
}
