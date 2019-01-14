using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a test that handles a specific message with a <see cref="IMicroProcessor" />
    /// and produces a set of events as a result.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this test.</typeparam>
    /// <typeparam name="TEventStream">Type of the event-stream that is produced by this test.</typeparam>
    public interface IHandleMessageTest<TMessage, out TEventStream> : IMicroProcessorTest<TMessage>
        where TEventStream : EventStream
    {
        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        /// <param name="result">The result of this test.</param>        
        void Then(TMessage message, MicroProcessorTestContext context, IHandleMessageResult<TEventStream> result);
    }
}
