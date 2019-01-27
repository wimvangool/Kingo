using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a <see cref="IMessageHandlerTest{TMessage,TEventStream}"/>.
    /// </summary>
    /// <typeparam name="TEventStream">Type of the event-stream produced by the test.</typeparam>
    public interface IMessageHandlerResult<in TEventStream> : IMicroProcessorTestResult
        where TEventStream : EventStream
    {
        /// <summary>
        /// Asserts that the test produced a specific set of events.
        /// </summary>        
        /// <param name="assertion">
        /// Delegate to verify the details of all the published events and to create and return
        /// an event stream of type <typeparamref name="TEventStream" />.
        /// </param>
        /// <param name="expectedEventCount">
        /// Optional value that, if specified, represents the expected number of events in the event-stream.        
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assertion"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="expectedEventCount"/> is a negative number.
        /// </exception>
        void IsExpectedEventStream(Func<EventStream, TEventStream> assertion, int? expectedEventCount = null);
    }
}
