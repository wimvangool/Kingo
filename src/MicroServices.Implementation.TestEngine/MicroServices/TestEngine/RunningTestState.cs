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
                context.SetResult(operation.Id, await context.Processor.ExecuteCommandAsync(messageHandler, operation.Message));
            }
            return operation.Id;
        }

        public async Task<MicroProcessorTestOperationId> HandleEventAsync<TMessage>(MicroProcessorTestContext context, IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperationInfo<TMessage> operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                context.SetResult(operation.Id, await context.Processor.HandleEventAsync(messageHandler, operation.Message));
            }
            return operation.Id;
        }

        #endregion

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
                throw NewGivenOperationFailedException(operation, exception);
            }
        }

        private static Exception NewGivenOperationFailedException(MicroProcessorTestOperation operation, Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_GivenOperatonFailed;
            var message = string.Format(messageFormat, operation, exception.GetType().FriendlyName());
            return new TestFailedException(message, exception);
        }
    }
}
