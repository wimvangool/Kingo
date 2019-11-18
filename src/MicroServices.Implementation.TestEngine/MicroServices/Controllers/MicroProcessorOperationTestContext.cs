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
        private readonly Dictionary<IMicroProcessorOperationTest, object> _testResults;
        private readonly IMicroProcessor _processor;        

        internal MicroProcessorOperationTestContext(IMicroProcessor processor)
        {                        
            _testResults = new Dictionary<IMicroProcessorOperationTest, object>();
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
            $"{_testResults.Count} output-stream(s) stored.";        

        #region [====== SetTestResult ======]

        internal void SetTestResult<TMessage, TOutputStream>(IMicroProcessorOperationTest test, MessageEnvelope<TMessage> message, TOutputStream outputStream) where TOutputStream : MessageStream
        {
            try
            {
                _testResults.Add(test, Tuple.Create(message, outputStream));
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

        #region [====== GetTestResult ======]     

        /// <summary>
        /// Returns the message that was processed by the specified <paramref name="test"/>.
        /// </summary>        
        /// <param name="test">The test that produced the message-stream.</param>
        /// <returns>The message-stream that was stored in this context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.        
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No message-stream produced by the specified <paramref name="test"/> was stored in this context.
        /// </exception>
        public MessageEnvelope<TMessage> GetInputMessage<TMessage, TOutputStream>(IMessageHandlerOperationTest<TMessage, TOutputStream> test) where TOutputStream : MessageStream =>
            GetTestResult(test).Item1;

        /// <summary>
        /// Returns the <see cref="MessageStream"/> that was produced by the specified <paramref name="test"/>.
        /// </summary>        
        /// <param name="test">The test that produced the message-stream.</param>
        /// <returns>The message-stream that was stored in this context.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="test"/> is <c>null</c>.        
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No message-stream produced by the specified <paramref name="test"/> was stored in this context.
        /// </exception>
        public TOutputStream GetOutputStream<TMessage, TOutputStream>(IMessageHandlerOperationTest<TMessage, TOutputStream> test) where TOutputStream : MessageStream =>
            GetTestResult(test).Item2;

        private Tuple<MessageEnvelope<TMessage>, TOutputStream> GetTestResult<TMessage, TOutputStream>(IMessageHandlerOperationTest<TMessage, TOutputStream> test) where TOutputStream : MessageStream
        {
            if (test == null)
            {
                throw new ArgumentNullException(nameof(test));
            }
            try
            {
                return (Tuple<MessageEnvelope<TMessage>, TOutputStream>) _testResults[test];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewTestResultNotFoundException(test, exception);
            }
        }

        private static Exception NewTestResultNotFoundException(object test, Exception innerException)            
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestResultNotFound;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new ArgumentException(message, nameof(test), innerException);
        }

        #endregion                
    }
}
