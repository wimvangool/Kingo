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
        /// <param name="expectedEventCount">The expected number of events that were published.</param>
        /// <param name="assertion">
        /// Delegate to verify the details of all the published events and to create and return
        /// an event stream of type <typeparamref name="TEventStream" />.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="expectedEventCount"/> is a negative number.
        /// </exception>
        void IsExpectedEventStream(int expectedEventCount, Func<EventStream, TEventStream> assertion);
    }
}
