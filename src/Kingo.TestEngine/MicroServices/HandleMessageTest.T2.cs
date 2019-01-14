using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's that handle a message and return the resulting event stream.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this test.</typeparam>
    /// <typeparam name="TEventStream">Type of the resulting event stream.</typeparam>
    public abstract class HandleMessageTest<TMessage, TEventStream> : MicroProcessorTest<TMessage>, IHandleMessageTest<TMessage, TEventStream>
        where TEventStream : EventStream
    {
        void IHandleMessageTest<TMessage, TEventStream>.Then(TMessage message, MicroProcessorTestContext context, IHandleMessageResult<TEventStream> result) =>
            Then(message, context, result);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        /// <param name="result">The result of this test.</param>  
        protected abstract void Then(TMessage message, MicroProcessorTestContext context, IHandleMessageResult<TEventStream> result);
    }
}
