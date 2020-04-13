using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kingo.Clocks;
using Kingo.Reflection;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class RunningTestState : MicroProcessorTestState
    {
        #region [====== GivenOperation ======]

        private sealed class GivenOperation : MicroProcessorTestOperation
        {
            private readonly MicroProcessorTestOperation _operation;

            public GivenOperation(MicroProcessorTestOperation operation)
            {
                _operation = operation;
            }

            public override async Task<MicroProcessorTestOperationId> RunAsync(RunningTestState state, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context)
            {
                try
                {
                    return await _operation.RunAsync(state, nextOperations, context);
                }
                catch (TestFailedException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw NewOperationFailedException(_operation, exception);
                }
            }
        }

        #endregion

        private readonly MicroProcessorTest _test;
        private readonly IEnumerable<MicroProcessorTestOperation> _givenOperations;

        protected RunningTestState(MicroProcessorTest test, IEnumerable<MicroProcessorTestOperation> givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        #region [====== RunOperationsAsync ======]

        protected Task<MicroProcessorTestOperationId> RunOperationsAsync(MicroProcessorTestOperation whenOperation, MicroProcessorTestContext context) =>
            RunOperationsAsync(CreateOperationQueue(_givenOperations, whenOperation), context);

        private async Task<MicroProcessorTestOperationId> RunOperationsAsync(Queue<MicroProcessorTestOperation> operations, MicroProcessorTestContext context)
        {
            var operationId = MicroProcessorTestOperationId.Empty;

            while (operations.Count > 0)
            {
                operationId = await operations.Dequeue().RunAsync(this, operations, context);
            }
            return operationId;
        }

        private static Queue<MicroProcessorTestOperation> CreateOperationQueue(IEnumerable<MicroProcessorTestOperation> givenOperations, MicroProcessorTestOperation whenOperation)
        {
            var operations = new Queue<MicroProcessorTestOperation>(givenOperations.Select(operation => new GivenOperation(operation)));
            operations.Enqueue(whenOperation);
            return operations;
        }

        protected static Exception NewOperationFailedException(MicroProcessorTestOperation operation, Exception exception)
        {
            var messageFormat = ExceptionMessages.MicroProcessorTest_OperatonFailed;
            var message = string.Format(messageFormat, operation, exception.GetType().FriendlyName());
            return new TestFailedException(message, exception);
        }

        #endregion

        #region [====== Time Operations ======]

        public async Task<MicroProcessorTestOperationId> SetClockToSpecificTimeAsync(DateTimeOffset value, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context)
        {
            using (context.Processor.AssignClock(clock => clock.SetToSpecificTime(value)))
            {
                return await RunOperationsAsync(nextOperations, context);
            }
        }

        public async Task<MicroProcessorTestOperationId> ShiftClockBySpecificOffsetAsync(TimeSpan offset, Queue<MicroProcessorTestOperation> nextOperations, MicroProcessorTestContext context)
        {
            using (context.Processor.AssignClock(clock => clock.Shift(offset)))
            {
                return await RunOperationsAsync(nextOperations, context);
            }
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
