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
        #region [====== MessageHandlerOperattionResult ======]

        private sealed class MessageHandlerOperationResult
        {
            public MessageHandlerOperationResult(IMessageEnvelope inputMessage, MessageStream outputStream)
            {
                InputMessage = inputMessage;
                OutputStream = outputStream;
            }

            public IMessageEnvelope InputMessage
            {
                get;
            }

            public MessageStream OutputStream
            {
                get;
            }

            public override string ToString() =>
                $"{InputMessage.Content.GetType().FriendlyName()} --> {OutputStream.Count} message(s)";
        }

        #endregion

        private readonly Dictionary<Guid, MessageHandlerOperationResult> _results;
        private readonly IMicroProcessor _processor;        

        internal MicroProcessorOperationTestContext(IMicroProcessor processor)
        {                        
            _results = new Dictionary<Guid, MessageHandlerOperationResult>();
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
            $"{_results.Count} operation(s) executed.";

        #region [====== SetResult ======]

        internal MessageStream SetResult<TMessage>(Guid operationId, MessageHandlerOperationResult<TMessage> result)
        {
            var outputStream = new MessageStream(result.Output);

            try
            {
                _results.Add(operationId, new MessageHandlerOperationResult(result.Input, outputStream));
            }
            catch (ArgumentException exception)
            {
                throw NewOperationAlreadyExecutedException(operationId, exception);
            }
            return outputStream;
        }

        private static Exception NewOperationAlreadyExecutedException(object test, Exception innerException)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestAlreadyRun;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new InvalidOperationException(message, innerException);
        }

        #endregion

        #region [====== GetResult ======]     

        internal MessageEnvelope<TMessage> GetInputMessage<TMessage>(Guid operationId) =>
            (MessageEnvelope<TMessage>) GetResult(operationId).InputMessage;

        internal MessageStream GetOutputStream(Guid operationId) =>
            GetResult(operationId).OutputStream;

        private MessageHandlerOperationResult GetResult(Guid operationId)
        {
            try
            {
                return _results[operationId];
            }
            catch (KeyNotFoundException exception)
            {
                throw NewOperationNotExecutedException(operationId, exception);
            }
        }

        private static Exception NewOperationNotExecutedException(Guid operationId, Exception innerException)            
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestResultNotFound;
            var message = string.Format(messageFormat, operationId);
            return new ArgumentException(message, nameof(operationId), innerException);
        }

        #endregion                
    }
}
