using System;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class HandleMessageResult<TMessageStream> : MicroProcessorOperationTestResult, IHandleMessageResult<TMessageStream>
        where TMessageStream : MessageStream
    {
        #region [====== ExceptionResult ======]

        private sealed class ExceptionResult : MicroProcessorOperationTestResultBase, IHandleMessageResult<TMessageStream>
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

            public void IsMessageStream(Func<MessageStream, TMessageStream> assertion) =>
                throw NewExceptionThrownException(_exception);
        }

        #endregion

        #region [====== EventStreamResult ======]

        private sealed class EventStreamResult : MicroProcessorOperationTestResultBase, IHandleMessageResult<TMessageStream>
        {                        
            private readonly MessageStream _stream;
            private readonly Action<TMessageStream> _streamConsumer;

            public EventStreamResult(MessageStream stream, Action<TMessageStream> streamConsumer)
            {
                _stream = stream;
                _streamConsumer = streamConsumer;
            }

            public override string ToString() =>
                $"{nameof(MessageStream)} ({_stream})";

            public override IInnerExceptionResult IsExceptionOfType<TException>(Action<TException> assertion = null) =>
                throw NewExceptionNotThrownException(typeof(TException));

            public void IsMessageStream(Func<MessageStream, TMessageStream> assertion)
            {
                if (assertion == null)
                {
                    throw new ArgumentNullException(nameof(assertion));
                }
                _streamConsumer.Invoke(AssertAndConvert(_stream, assertion));
            }                                    
            
            private static TMessageStream AssertAndConvert(MessageStream stream, Func<MessageStream, TMessageStream> assertion)
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
                var messageFormat = ExceptionMessages.MessageHandlerResult_AssertionOfMessageStreamFailed;
                var message = string.Format(messageFormat, typeof(TMessageStream).FriendlyName());
                throw new TestFailedException(message, innerException);
            }
        }

        #endregion

        private readonly IHandleMessageResult<TMessageStream> _result;

        public HandleMessageResult(Exception exception)
        {
            _result = new ExceptionResult(exception);
        }

        public HandleMessageResult(MessageStream stream, Action<TMessageStream> streamConsumer)
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

        public void IsMessageStream(Func<MessageStream, TMessageStream> assertion)
        {
            try
            {
                _result.IsMessageStream(assertion);
            }
            finally
            {
                OnVerified();
            }
        }                    
    }
}
