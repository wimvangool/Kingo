using System;
using System.Collections.Generic;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.TestEngine
{
    /// <summary>
    /// When implemented by a class, represents the context in which a test executes.
    /// </summary>
    public sealed class MicroProcessorTestContext
    {
        #region [====== TestOperationResult<TOutput> ======]

        private abstract class TestOperationResult
        {
            public abstract IMessage Input
            {
                get;
            }

            protected virtual string FormatInput() =>
                Format(Input);

            protected static string Format(IMessage message) =>
                message.Content.GetType().FriendlyName();
        }

        #endregion

        #region [====== TestOperationResult<TOutput> ======]

        private abstract class TestOperationResult<TOutput> : TestOperationResult
        {
            public abstract TOutput Output
            {
                get;
            }

            protected abstract string FormatOutput();

            public override string ToString() =>
                $"{FormatInput()} --> {FormatOutput()}";
        }

        #endregion

        #region [====== MessageHandlerTestOperationResult ======]

        private sealed class MessageHandlerTestOperationResult : TestOperationResult<MessageStream>
        {
            public MessageHandlerTestOperationResult(IMessage input, MessageStream output)
            {
                Input = input;
                Output = output;
            }

            public override IMessage Input
            {
                get;
            }

            public override MessageStream Output
            {
                get;
            }

            protected override string FormatOutput() =>
                $"{Output.Count} message(s)";
        }

        #endregion

        #region [====== QueryTestOperationResult ======]

        private sealed class QueryTestOperationResult<TResponse> : TestOperationResult<Message<TResponse>>
        {
            public QueryTestOperationResult(IMessage input, Message<TResponse> output)
            {
                Input = input;
                Output = output;
            }

            public override IMessage Input
            {
                get;
            }

            public override Message<TResponse> Output
            {
                get;
            }

            protected override string FormatInput() =>
                Input == null ? "Void" : base.FormatInput();

            protected override string FormatOutput() =>
                Format(Output);
        }

        #endregion

        private readonly Dictionary<MicroProcessorTestOperationId, TestOperationResult> _results;
        private readonly MicroProcessorTest _test;
        private readonly IMicroProcessor _processor;        

        internal MicroProcessorTestContext(MicroProcessorTest test, IMicroProcessor processor)
        {                        
            _results = new Dictionary<MicroProcessorTestOperationId, TestOperationResult>();
            _test = test;
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
                configurator?.Invoke(operation, this);

                if (operation.Id.Equals(MicroProcessorTestOperationId.Empty))
                {
                    operation.Id = MicroProcessorTestOperationId.NewOperationId();
                }
                if (operation.User == null)
                {
                    operation.User = _test.User;
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

        internal TMessageHandlerOrQuery Resolve<TMessageHandlerOrQuery>()
        {
            try
            {
                return ServiceProvider.GetRequiredService<TMessageHandlerOrQuery>();
            }
            catch (Exception exception)
            {
                throw NewCouldNotResolveComponentException(typeof(TMessageHandlerOrQuery), exception);
            }
        }

        /// <summary>
        /// The service provider that is used to resolve dependencies during test execution.
        /// </summary>
        public IServiceProvider ServiceProvider =>
            _processor.ServiceProvider;

        private static Exception NewCouldNotResolveComponentException(Type messageHandlerType, Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_CouldNotResolveComponent;
            var message = string.Format(messageFormat, messageHandlerType.FriendlyName());
            return new TestFailedException(message, exception);
        }

        #endregion

        #region [====== SetResult ======]

        internal MicroProcessorTestOperationId SetResult<TMessage>(MicroProcessorTestOperationId operationId, MessageHandlerOperationResult<TMessage> result) =>
            SetResult(operationId, new MessageHandlerTestOperationResult(result.Input, new MessageStream(result.Output)));

        internal MicroProcessorTestOperationId SetResult<TResponse>(MicroProcessorTestOperationId operationId, QueryOperationResult<TResponse> result) =>
            SetResult(operationId, new QueryTestOperationResult<TResponse>(null, result.Output));

        internal MicroProcessorTestOperationId SetResult<TRequest, TResponse>(MicroProcessorTestOperationId operationId, QueryOperationResult<TRequest, TResponse> result) =>
            SetResult(operationId, new QueryTestOperationResult<TResponse>(result.Input, result.Output));

        private MicroProcessorTestOperationId SetResult(MicroProcessorTestOperationId operationId, TestOperationResult result)
        {
            try
            {
                _results.Add(operationId, result);
            }
            catch (ArgumentException exception)
            {
                throw NewOperationAlreadyExecutedException(operationId, exception);
            }
            return operationId;
        }

        private static Exception NewOperationAlreadyExecutedException(object test, Exception innerException)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestAlreadyRun;
            var message = string.Format(messageFormat, test.GetType().FriendlyName());
            return new InvalidOperationException(message, innerException);
        }

        #endregion

        #region [====== GetResult ======]     

        internal Message<TMessage> GetInputMessage<TMessage>(MicroProcessorTestOperationId operationId) =>
            (Message<TMessage>) GetOperationResult(operationId).Input;

        internal MessageStream GetOutputStream(MicroProcessorTestOperationId operationId) =>
            GetOperationResult<MessageHandlerTestOperationResult>(operationId).Output;

        internal Message<TResponse> GetResponse<TResponse>(MicroProcessorTestOperationId operationId) =>
            GetOperationResult<QueryTestOperationResult<TResponse>>(operationId).Output;

        private TOperationResult GetOperationResult<TOperationResult>(MicroProcessorTestOperationId operationId) where TOperationResult : TestOperationResult =>
            (TOperationResult) GetOperationResult(operationId);

        private TestOperationResult GetOperationResult(MicroProcessorTestOperationId operationId)
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

        private static Exception NewOperationNotExecutedException(MicroProcessorTestOperationId operationId, Exception innerException)            
        {
            var messageFormat = ExceptionMessages.MicroProcessorTestContext_TestResultNotFound;
            var message = string.Format(messageFormat, operationId);
            return new ArgumentException(message, nameof(operationId), innerException);
        }

        #endregion

        #region [====== ToConfigurator ======]

        internal static Action<MessageHandlerTestOperationInfo<TMessage>, MicroProcessorTestContext> ConfigureMessage<TMessage>(TMessage message)
        {
            return (operation, context) =>
            {
                operation.Message = message;
            };
        }

        internal static Action<QueryTestOperationInfo<TRequest>, MicroProcessorTestContext> ConfigureRequest<TRequest>(TRequest request)
        {
            return (operation, context) =>
            {
                operation.Request = request;
            };
        }

        #endregion
    }
}
