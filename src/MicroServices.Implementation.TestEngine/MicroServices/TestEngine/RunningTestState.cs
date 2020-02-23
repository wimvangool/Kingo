using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Clocks;

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

        #region [====== Given ======]

        public async Task SetClockToSpecificTimeAsync(MicroProcessorTestContext context, DateTimeOffset value)
        {
            using (Clock.OverrideAsyncLocal(Clock.Current.SetToSpecificTime(value)))
            {
                await GivenAsync(context);
            }
        }

        public async Task ShiftClockBySpecificPeriodAsync(MicroProcessorTestContext context, TimeSpan value)
        {
            using (Clock.OverrideAsyncLocal(Clock.Current.Shift(value)))
            {
                await GivenAsync(context);
            }
        }

        protected async Task GivenAsync(MicroProcessorTestContext context)
        {
            while (_operations.Count > 0)
            {
                await _operations.Dequeue().RunAsync(this, context);
            }
        }

        #endregion

        #region [====== Commands & Events ======]

        public async Task ExecuteCommandAsync<TMessage>(MicroProcessorTestContext context, IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperationInfo<TMessage> operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                context.SetResult(operation.Id, await context.Processor.ExecuteCommandAsync(messageHandler, operation.Message));
            }
        }

        public async Task HandleEventAsync<TMessage>(MicroProcessorTestContext context, IMessageHandler<TMessage> messageHandler, MessageHandlerTestOperationInfo<TMessage> operation)
        {
            using (context.Processor.AssignUser(operation.User))
            {
                context.SetResult(operation.Id, await context.Processor.HandleEventAsync(messageHandler, operation.Message));
            }
        }

        #endregion
    }
}
