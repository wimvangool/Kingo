using System;
using System.Collections.Generic;
using Kingo.Reflection;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents the context in which a test executes.
    /// </summary>
    public sealed class MicroProcessorOperationTestContext
    {        
        private readonly Dictionary<IMicroProcessorOperationTest, MessageStream> _messageStreams;
        private readonly IMicroProcessor _processor;        

        internal MicroProcessorOperationTestContext(IMicroProcessor processor)
        {                        
            _messageStreams = new Dictionary<IMicroProcessorOperationTest, MessageStream>();
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
            $"{_messageStreams.Count} message stream(s) stored.";        

        #region [====== SetMessageStream ======]

        internal void SetMessageStream<TMessageStream>(IMicroProcessorOperationTest test, TMessageStream stream) where TMessageStream : MessageStream
        {
            try
            {
                _messageStreams.Add(test, stream);
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

        #region [====== GetMessageStream ======]        

        /// <summary>
        /// Returns the <see cref="MessageStream"/> that was produced by the specified <paramref name="test"/> and stored in this context.
        /// </summary>        
        /// <param name="test">The test that produced the message-stream.</param>
        /// <returns>The message-stream that was stored in this context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.        
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No message-stream produced by the specified <paramref name="test"/> was stored in this context.
        /// </exception>
        public TMessageStream GetMessageStream<TMessage, TMessageStream>(IHandleMessageTest<TMessage, TMessageStream> test) where TMessageStream : MessageStream
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }
            try
            {
                return (TMessageStream) _messageStreams[test];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewMessageStreamNotFoundException(test, exception);
            }            
        }

        private static Exception NewMessageStreamNotFoundException(object test, Exception innerException)            
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_MessageStreamNotFound;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new ArgumentException(message, nameof(test), innerException);
        }

        #endregion                
    }
}
