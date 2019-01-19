using System;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerResult<TEventStream> : MicroProcessorTestResult, IMessageHandlerResult<TEventStream>
        where TEventStream : EventStream
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorTestResult, IMessageHandlerResult<TEventStream>
        {
            private readonly Exception _exception;

            public ExceptionResult(Exception exception)
            {                
                _exception = exception;
            }

            public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
                IsExpectedException(_exception, assertion);

            public void IsExpectedEventStream(Func<EventStream, TEventStream> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== EventStreamResult ======]

        private sealed class EventStreamResult : MicroProcessorTestResult, IMessageHandlerResult<TEventStream>
        {                        
            private readonly EventStream _stream;
            private readonly Action<TEventStream> _streamConsumer;

            public EventStreamResult(EventStream stream, Action<TEventStream> streamConsumer)
            {
                _stream = stream;
                _streamConsumer = streamConsumer;
            }

            public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsExpectedEventStream(Func<EventStream, TEventStream> assertion)
            {
                if (assertion == null)
                {
                    throw new ArgumentNullException(nameof(assertion));
                }
                _streamConsumer.Invoke(assertion.Invoke(_stream));
            }            
        }

        #endregion

        private readonly IMessageHandlerResult<TEventStream> _result;

        public MessageHandlerResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public MessageHandlerResult(EventStream stream, Action<TEventStream> streamConsumer)
        {
            _result = new EventStreamResult(stream, streamConsumer);
        }

        public override void IsExpectedException<TException>(Action<TException> assertion = null) =>
            _result.IsExpectedException(assertion);

        public void IsExpectedEventStream(Func<EventStream, TEventStream> assertion = null) =>
            _result.IsExpectedEventStream(assertion);       
    }
}
