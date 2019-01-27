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

            public void IsExpectedEventStream(Func<EventStream, TEventStream> assertion, int? expectedEventCount = null) =>
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

            public void IsExpectedEventStream(Func<EventStream, TEventStream> assertion, int? expectedEventCount = null)
            {
                if (assertion == null)
                {
                    throw new ArgumentNullException(nameof(assertion));
                }
                if (TryGetExpectedEventCount(expectedEventCount, out var eventCount) && _stream.Count != eventCount)
                {
                    throw NewUnexpectedEventCountException(eventCount, _stream.Count);
                }
                _streamConsumer.Invoke(AssertAndConvert(_stream, assertion));
            }

            private static Exception NewUnexpectedEventCountException(int expectedEventCount, int actualEventCount)
            {
                var messageFormat = ExceptionMessages.MessageHandlerResult_UnexpectedEventCount;
                var message = string.Format(messageFormat, expectedEventCount, actualEventCount);
                return new AssertFailedException(message);
            }

            private static bool TryGetExpectedEventCount(int? expectedEventCount, out int eventCount)
            {
                if (expectedEventCount.HasValue)
                {
                    if ((eventCount = expectedEventCount.Value) < 0)
                    {
                        throw NewInvalidEventCountException(eventCount);
                    }
                    return true;
                }
                eventCount = 0;
                return false;
            }

            private static Exception NewInvalidEventCountException(int expectedEventCount)
            {
                var messageFormat = ExceptionMessages.MessageHandlerResult_InvalidEventCountSpecified;
                var message = string.Format(messageFormat, expectedEventCount);
                return new ArgumentOutOfRangeException(nameof(expectedEventCount), message);
            }  
            
            private static TEventStream AssertAndConvert(EventStream stream, Func<EventStream, TEventStream> assertion)
            {
                try
                {
                    return assertion.Invoke(stream);
                }
                catch (Exception exception)
                {
                    var messageFormat = ExceptionMessages.MessageHandlerResult_UnexpectedStreamContents;
                    var message = string.Format(messageFormat, typeof(TEventStream).FriendlyName());
                    throw new AssertFailedException(message, exception);
                }
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

        public void IsExpectedEventStream(Func<EventStream, TEventStream> assertion, int? expectedEventCount = null) =>
            _result.IsExpectedEventStream(assertion, expectedEventCount);       
    }
}
