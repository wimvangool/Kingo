using System;

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

            public void IsExpectedEventStream(Action<EventStream> assertion = null)
            {
                _result.IsExpectedEventStream(stream =>
                {
                    assertion?.Invoke(stream);
                    return stream;
                });
            }
        }

        #endregion

        /// <inheritdoc />
        protected sealed override void Then(TMessage message, IHandleMessageResult<EventStream> result, MicroProcessorTestContext context) =>
            Then(message, new HandleMessageResult(result), context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        protected abstract void Then(TMessage message, IHandleMessageResult result, MicroProcessorTestContext context);
    }
}
