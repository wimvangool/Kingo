﻿using System;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Serves as a base-class for all tests that handle a message and return an empty stream or throw an exception.
    /// </summary>
    public abstract class HandleMessageTest<TMessage> : HandleMessageTest<TMessage, MessageStream>
    {
        #region [====== HandleMessageResult ======]

        private sealed class HandleMessageResult : IHandleMessageResult
        {
            private readonly IHandleMessageResult<MessageStream> _result;

            public HandleMessageResult(IHandleMessageResult<MessageStream> result)
            {
                _result = result;
            }

            public IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) where TException : Exception =>
                _result.IsExceptionOfType(assertion);

            public void IsMessageStream(Action<MessageStream> assertion = null)
            {
                _result.IsMessageStream(stream =>
                {
                    assertion?.Invoke(stream);
                    return stream;
                });
            }
        }

        #endregion

        /// <inheritdoc />
        protected sealed override void Then(TMessage message, IHandleMessageResult<MessageStream> result, MicroProcessorOperationTestContext context) =>
            Then(message, new HandleMessageResult(result), context);

        /// <summary>
        /// Verifies the <paramref name="result"/> of this test.
        /// </summary>
        /// <param name="message">The message that was handled by this test.</param>        
        /// <param name="result">The result of this test.</param>
        /// <param name="context">The context in which the test is running.</param>                
        protected abstract void Then(TMessage message, IHandleMessageResult result, MicroProcessorOperationTestContext context);
    }
}
