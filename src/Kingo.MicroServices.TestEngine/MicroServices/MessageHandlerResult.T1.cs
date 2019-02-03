using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MessageHandlerResult<TEventStream> : MicroProcessorTestResult, IMessageHandlerResult<TEventStream>
        where TEventStream : EventStream
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorTestResultBase, IMessageHandlerResult<TEventStream>
        {
            private readonly Exception _exception;            

            public ExceptionResult(Exception exception)
            {                
                _exception = exception;
            }

            public override string ToString() =>
                _exception.GetType().FriendlyName();

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                IsExpectedException(_exception, assertion);

            public void IsEventStream(Func<EventStream, TEventStream> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== EventStreamResult ======]

        private sealed class EventStreamResult : MicroProcessorTestResultBase, IMessageHandlerResult<TEventStream>
        {                        
            private readonly EventStream _stream;
            private readonly Action<TEventStream> _streamConsumer;

            public EventStreamResult(EventStream stream, Action<TEventStream> streamConsumer)
            {
                _stream = stream;
                _streamConsumer = streamConsumer;
            }

            public override string ToString() =>
                $"{nameof(EventStream)} ({_stream})";

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsEventStream(Func<EventStream, TEventStream> assertion)
            {
                if (assertion == null)
                {
                    throw new ArgumentNullException(nameof(assertion));
                }
                _streamConsumer.Invoke(AssertAndConvert(_stream, assertion));
            }                                    
            
            private static TEventStream AssertAndConvert(EventStream stream, Func<EventStream, TEventStream> assertion)
            {
                try
                {
                    return assertion.Invoke(stream);
                }
                catch (Exception innerException)
                {
                    throw NewAssertionOfEventStreamFailedException(innerException);
                }
            }

            private static Exception NewAssertionOfEventStreamFailedException(Exception innerException)
            {
                var messageFormat = ExceptionMessages.MessageHandlerResult_AssertionOfEventStreamFailed;
                var message = string.Format(messageFormat, typeof(TEventStream).FriendlyName());
                throw new TestFailedException(message, innerException);
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

        public override string ToString() =>
            _result.ToString();

        public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null)
        {
            try
            {
                return _result.IsExceptionOfType(assertion);
            }
            finally
            {
                OnVerified();
            }
        }            

        public void IsEventStream(Func<EventStream, TEventStream> assertion)
        {
            try
            {
                _result.IsEventStream(assertion);
            }
            finally
            {
                OnVerified();
            }
        }                    
    }
}
