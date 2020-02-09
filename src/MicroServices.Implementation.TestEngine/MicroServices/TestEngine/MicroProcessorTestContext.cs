using System;
using System.Collections.Generic;
using System.Security.Claims;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the context in which a test executes.
    /// </summary>
    public sealed class MicroProcessorTestContext
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

        internal MicroProcessorTestContext(IMicroProcessor processor)
        {                        
            _results = new Dictionary<Guid, MessageHandlerOperationResult>();
            _processor = processor;            
        }

        internal IMicroProcessor Processor =>
            _processor;

        /// <inheritdoc />
        public override string ToString() =>
            $"{_results.Count} operation(s) executed.";

        #region [====== ConfigureOperation ======]

        internal MessageHandlerTestOperationInfo<TMessage> CreateOperationInfo<TMessage>(Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> configurator)
        {
            var operation = CreateOperationInfo<MessageHandlerTestOperationInfo<TMessage>>(configurator);
            if (operation.Message is null)
            {
                throw NewMessageNotSetException(typeof(TMessage));
            }
            return operation;
        }

        internal QueryTestOperationInfo CreateOperationInfo(Action<QueryTestOperationInfo, MicroProcessorTestContext> configurator) =>
            CreateOperationInfo<QueryTestOperationInfo>(configurator);

        internal QueryTestOperationInfo<TRequest> CreateOperationInfo<TRequest>(Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> configurator)
        {
            var operation = CreateOperationInfo<QueryTestOperationInfo<TRequest>>(configurator);
            if (operation.Request is null)
            {
                throw NewRequestNotSetException(typeof(TRequest));
            }
            return operation;
        }

        private TOperationInfo CreateOperationInfo<TOperationInfo>(Action<TOperationInfo, MicroProcessorTestContext> configurator) where TOperationInfo : MicroProcessorTestOperationInfo, new()
        {
            var operation = new TOperationInfo();

            try
            {
                configurator.Invoke(operation, this);

                if (operation.Id.Equals(MicroProcessorTestOperationId.Empty))
                {
                    operation.Id = MicroProcessorTestOperationId.NewOperationId();
                }
                if (operation.User == null)
                {
                    operation.User = ClaimsPrincipal.Current;
                }
                return operation;
            }
            catch (TestFailedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewConfigureOperationFailedException(exception);
            }
        }

        private static Exception NewConfigureOperationFailedException(Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_ConfigureOperationFailed;
            var message = string.Format(messageFormat, exception.GetType().FriendlyName());
            return new TestFailedException(message, exception);
        }

        private static Exception NewMessageNotSetException(Type messageType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_MessageNotSet;
            var message = string.Format(messageFormat, messageType.FriendlyName());
            return new TestFailedException(message);
        }

        private static Exception NewRequestNotSetException(Type requestType)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_RequestNotSet;
            var message = string.Format(messageFormat, requestType.FriendlyName());
            return new TestFailedException(message);
        }

        #endregion

        #region [====== ServiceProvider ======]

        internal TMessageHandler Resolve<TMessageHandler>()
        {
            try
            {
                return ServiceProvider.GetRequiredService<TMessageHandler>();
            }
            catch (Exception exception)
            {
                throw NewCouldNotResolveMessageHandlerException(typeof(TMessageHandler), exception);
            }
        }

        /// <summary>
        /// The service provider that is used to resolve dependencies during test execution.
        /// </summary>
        public IServiceProvider ServiceProvider =>
            _processor.ServiceProvider;

        private static Exception NewCouldNotResolveMessageHandlerException(Type messageHandlerType, Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_CouldNotResolveMessageHandler;
            var message = string.Format(messageFormat, messageHandlerType.FriendlyName());
            return new TestFailedException(message, exception);
        }

        #endregion

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
