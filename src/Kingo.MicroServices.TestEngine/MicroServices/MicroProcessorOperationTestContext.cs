using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the context in which a test executes.
    /// </summary>
    public sealed class MicroProcessorOperationTestContext
    {        
        private readonly Dictionary<IMicroProcessorOperationTest, EventStream> _eventStreams;
        private readonly IMicroProcessor _processor;        

        internal MicroProcessorOperationTestContext(IMicroProcessor processor)
        {                        
            _eventStreams = new Dictionary<IMicroProcessorOperationTest, EventStream>();
            _processor = processor;            
        }

        internal IMicroProcessor Processor =>
            _processor;

        /// <summary>
        /// The service provider that is used to resolve dependencies during test execution.
        /// </summary>
        public IServiceProvider ServiceProvider =>
            _processor.ServiceProvider;

        /// <inheritdoc />
        public override string ToString() =>
            $"{_eventStreams.Count} event stream(s) stored.";        

        #region [====== SetEventStream ======]

        internal void SetEventStream<TEventStream>(IMicroProcessorOperationTest test, TEventStream stream) where TEventStream : EventStream
        {
            try
            {
                _eventStreams.Add(test, stream);
            }
            catch (ArgumentException exception)
            {
                throw NewTestAlreadyRunException(test, exception);
            }
        }        

        private static Exception NewTestAlreadyRunException(object test, Exception innerException)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestAlreadyRun;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new InvalidOperationException(message, innerException);
        }

        #endregion

        #region [====== GetEventStream ======]        

        /// <summary>
        /// Returns the <see cref="EventStream"/> that was produced by the specified <paramref name="test"/> and stored in this context.
        /// </summary>        
        /// <param name="test">The test that produced the event-stream.</param>
        /// <returns>The event-stream that was stored in this context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.        
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No event-stream produced by the specified <paramref name="test"/> was stored in this context.
        /// </exception>
        public TEventStream GetEventStream<TMessage, TEventStream>(IHandleMessageTest<TMessage, TEventStream> test) where TEventStream : EventStream
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }
            try
            {
                return (TEventStream) _eventStreams[test];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewEventStreamNotFoundException(test, exception);
            }            
        }

        private static Exception NewEventStreamNotFoundException(object test, Exception innerException)            
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_EventStreamNotFound;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new ArgumentException(message, nameof(test), innerException);
        }

        #endregion                
    }
}
