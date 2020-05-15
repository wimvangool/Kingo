using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class ExecuteInternalFunctionTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== HighLevelFunction ======]

        private sealed class HighLevelFunction : IMessageHandler<int>
        {
            private readonly IMidLevelFunction _midLevelFunction;

            public HighLevelFunction(IMidLevelFunction midLevelFunction)
            {
                _midLevelFunction = midLevelFunction;
            }

            public async Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                Assert.AreEqual(1, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.MessageHandlerOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.RootOperation, context.StackTrace.CurrentOperation.Kind);
                Assert.AreEqual(MessageDirection.Input, context.StackTrace.CurrentOperation.Message.Direction);

                await _midLevelFunction.ExecuteAsync(2 * message, context);
            }
        }

        #endregion

        #region [====== MidLevelFunction ======]

        private interface IMidLevelFunction : IMessageHandler<int>
        {
            Task ExecuteAsync(int message, MessageHandlerOperationContext context) =>
                context.CommandProcessor.ExecuteCommandAsync(this, message);
        }

        private sealed class MidLevelFunction : IMidLevelFunction
        {
            private readonly ILowLevelFunction _lowLevelFunction;

            public MidLevelFunction(ILowLevelFunction lowLevelFunction)
            {
                _lowLevelFunction = lowLevelFunction;
            }

            public async Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                Assert.AreEqual(2, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.MessageHandlerOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.BranchOperation, context.StackTrace.CurrentOperation.Kind);
                Assert.AreEqual(MessageDirection.Internal, context.StackTrace.CurrentOperation.Message.Direction);

                await _lowLevelFunction.ExecuteAsync(3 * message, context);
            }
        }

        #endregion

        #region [====== LowLevelFunction ======]

        private interface ILowLevelFunction : IMessageHandler<int>
        {
            Task ExecuteAsync(int message, MessageHandlerOperationContext context) =>
                context.CommandProcessor.ExecuteCommandAsync(this, message);
        }

        private sealed class LowLevelFunction : ILowLevelFunction
        {
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                Assert.AreEqual(3, context.StackTrace.Count);
                Assert.AreEqual(MicroProcessorOperationType.MessageHandlerOperation, context.StackTrace.CurrentOperation.Type);
                Assert.AreEqual(MicroProcessorOperationKind.BranchOperation, context.StackTrace.CurrentOperation.Kind);
                Assert.AreEqual(MessageDirection.Internal, context.StackTrace.CurrentOperation.Message.Direction);

                context.MessageBus.Publish(4 * message);

                return Task.CompletedTask;
            }
        }

        #endregion

        private static readonly int _Value = DateTimeOffset.UtcNow.Second;

        [TestMethod]
        public async Task ExecuteCommandAsync_ExecutesInternalFunctionsAsExpected()
        {
            Processor.ConfigureMessageHandlers(messageHandlers =>
            {
                messageHandlers.Add<HighLevelFunction>();
                messageHandlers.Add<MidLevelFunction>();
                messageHandlers.Add<LowLevelFunction>();
            });

            var processor = CreateProcessor();
            var messageHandler = processor.ServiceProvider.GetRequiredService<HighLevelFunction>();
            var result = await processor.ExecuteCommandAsync(messageHandler, _Value);

            Assert.AreEqual(1, result.Output.Count);
            Assert.AreEqual(2 * 3 * 4 * _Value, result.Output[0].Content);
        }
    }
}
