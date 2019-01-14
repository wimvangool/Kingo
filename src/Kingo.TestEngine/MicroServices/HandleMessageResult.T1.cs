using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class HandleMessageResult<TEventStream> : IHandleMessageResult<TEventStream>
        where TEventStream : EventStream
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroServices.ExceptionResult, IHandleMessageResult<TEventStream>
        {            
            public ExceptionResult(MicroProcessorTestRunner testRunner, Exception exception)
            {
                TestRunner = testRunner;
                Exception = exception;
            }

            protected override MicroProcessorTestRunner TestRunner
            {
                get;
            }

            protected override Exception Exception
            {
                get;
            }

            public void IsExpectedEventStream(int expectedEventCount, Func<EventStream, TEventStream> assertion = null)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region [====== EventStreamResult ======]

        private sealed class EventStreamResult : NoExceptionResult, IHandleMessageResult<TEventStream>
        {
            public EventStreamResult(MicroProcessorTestRunner testRunner, TEventStream stream)
            {
                TestRunner = testRunner;
                Stream = stream;
            }

            protected override MicroProcessorTestRunner TestRunner
            {
                get;
            }

            private TEventStream Stream
            {
                get;
            }

            public void IsExpectedEventStream(int expectedEventCount, Func<EventStream, TEventStream> assertion = null)
            {
                throw new NotImplementedException();
            }            
        }

        #endregion

        private readonly IHandleMessageResult<TEventStream> _result;

        public HandleMessageResult(MicroProcessorTestRunner testRunner, Exception exception)
        {
            _result = new ExceptionResult(testRunner, exception);
        }

        public HandleMessageResult(MicroProcessorTestRunner testRunner, TEventStream stream)
        {
            _result = new EventStreamResult(testRunner, stream);
        }

        public void IsExpectedException<TException>(Action<TException> assertion = null) where TException : Exception =>
            _result.IsExpectedException(assertion);

        public void IsExpectedEventStream(int expectedEventCount, Func<EventStream, TEventStream> assertion = null) =>
            _result.IsExpectedEventStream(expectedEventCount, assertion);       
    }
}
