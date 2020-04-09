using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Clocks;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class RunningTestState : MicroProcessorTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly Queue<MicroProcessorTestOperation> _operations;

        protected RunningTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> operations)
        {
            _test = test;
            _operations = new Queue<MicroProcessorTestOperation>(operations);
        }

        protected override MicroProcessorTest Test =>
            _test;

        #region [====== GivenOperations ======]

        protected async Task RunGivenOperationsAsync(MicroProcessorTestContext context)
        {
            while (_operations.Count > 0)
            {
                await RunGivenOperationAsync(context, _operations.Dequeue());
            }
        }

        private async Task RunGivenOperationAsync(MicroProcessorTestContext context, MicroProcessorTestOperation operation)
        {
            try
            {
                await operation.RunAsync(this, context);
            }
            catch (TestFailedException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewOperationFailedException(operation, exception);
            }
        }

        protected static Exception NewOperationFailedException(MicroProcessorTestOperation operation, Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_OperatonFailed;
            var message = string.Format(messageFormat, operation, exception.GetType().FriendlyName());
            return new TestFailedException(message, exception);
        }

        #endregion

        #region [====== Time Operations ======]

        public async Task<MicroProcessorTestOperationId> SetClockToSpecificTimeAsync(MicroProcessorTestContext context, DateTimeOffset value)
        {
            using (Clock.OverrideAsyncLocal(Clock.Current.SetToSpecificTime(value)))
            {
                await RunGivenOperationsAsync(context);
            }
            return MicroProcessorTestOperationId.Empty;
        }

        public async Task<MicroProcessorTestOperationId> ShiftClockBySpecificPeriodAsync(MicroProcessorTestContext context, TimeSpan value)
        {
            using (Clock.OverrideAsyncLocal(Clock.Current.Shift(value)))
            {
                await RunGivenOperationsAsync(context);
            }
            return MicroProcessorTestOperationId.Empty;
        }

        #endregion

        #region [====== Command & Event Operations ======]

        public async Task<MicroProcessorTestOperationId> ExecuteCommandAsync<TMessage>(MicroProcessorTestContext context, IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperationInfo<TMessage> operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                return context.SetResult(operation.Id, await context.Processor.ExecuteCommandAsync(messageHandler, operation.Message));
            }
        }

        public async Task<MicroProcessorTestOperationId> HandleEventAsync<TMessage>(MicroProcessorTestContext context, IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperationInfo<TMessage> operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                return context.SetResult(operation.Id, await context.Processor.HandleEventAsync(messageHandler, operation.Message));
            }
        }

        #endregion

        #region [====== Query Operations ======]

        public async Task<MicroProcessorTestOperationId> ExecuteQuery<TResponse>(MicroProcessorTestContext context, IQuery<TResponse> query, QueryTestOperationInfo operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                return context.SetResult(operation.Id, await context.Processor.ExecuteQueryAsync(query));
            }
        }

        public async Task<MicroProcessorTestOperationId> ExecuteQuery<TRequest, TResponse>(MicroProcessorTestContext context, IQuery<TRequest, TResponse> query, QueryTestOperationInfo<TRequest> operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                return context.SetResult(operation.Id, await context.Processor.ExecuteQueryAsync(query, operation.Request));
            }
        }

        #endregion
    }
}
