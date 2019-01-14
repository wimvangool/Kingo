using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for all test's that handle a message and return an empty stream or throw an exception.
    /// </summary>
    public abstract class HandleMessageTest<TMessage> : HandleMessageTest<TMessage, EventStream>
    {
        #region [====== HandleMessageResult ======]

        private sealed class HandleMessageResult : IHandleMessageResult
        {
            private readonly IHandleMessageResult<EventStream> _result;

            public HandleMessageResult(IHandleMessageResult<EventStream> result)
            {
                _result = result;
            }

            public void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception =>
                _result.IsExpectedException(assertion);

            public void IsEmptyEventStream() =>
                _result.IsExpectedEventStream(0, stream => stream);
        }

        #endregion

        /// <inheritdoc />
        protected sealed override void Then(TMessage message, MicroProcessorTestContext context, IHandleMessageResult<EventStream> result) =>
            Then(message, context, new HandleMessageResult(result));

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        /// <param name="result">The result of this test.</param>  
        protected abstract void Then(TMessage message, MicroProcessorTestContext context, IHandleMessageResult result);
    }
}
