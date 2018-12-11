using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a result that is expected from processing a specific command or event.
    /// </summary>
    public interface IMessageHandlerTestResult : ITestResult
    {
        /// <summary>
        /// Asserts that the processor returns a specific stream of events when the message has been processed.
        /// </summary>        
        /// <param name="expectedEventCount">The amount of events that are expected to be published.</param>
        /// <param name="assertCallback">
        /// Optional delegate that can be used to assert the types and properties of the events in the stream.
        /// </param>
        /// <returns>A task that represents the execution of the associated scenario.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="expectedEventCount" /> is a negative value by the processor.
        /// </exception>
        Task IsEventStreamAsync(int expectedEventCount, Action<IReadOnlyList<object>> assertCallback = null);
    }
}
