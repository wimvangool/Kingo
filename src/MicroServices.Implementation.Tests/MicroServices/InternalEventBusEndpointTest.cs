using System;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class InternalEventBusEndpointTest : MicroProcessorTest<MicroProcessor>
    {
        [TestMethod]
        public async Task HandleAsync_DoesNotHandleMessage_IfMessageIsCommand()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<object>((message, context) =>
            {
                Fail(message);
            });

            var result = await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.MessageBus.Send(new object());
                context.MessageBus.Send(new object(), DateTimeOffset.Now);
            }, new object());

            Assert.AreEqual(1, result.MessageHandlerCount);
        }

        [TestMethod]
        public async Task HandleAsync_DoesNotHandleMessage_IfMessageIsEvent_But_EventIsScheduled()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<object>((message, context) =>
            {
                Fail(message);
            });

            var result = await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.MessageBus.Publish(new object(), DateTimeOffset.Now);
            }, new object());

            Assert.AreEqual(1, result.MessageHandlerCount);
        }

        [TestMethod]
        public async Task HandleAsync_HandlesMessages_IfMessageIsEvent_And_EventIsNotScheduled()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<object>((message, context) =>
            {
                Assert.AreEqual(2, context.StackTrace.Count);
            });

            var result = await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.MessageBus.Publish(new object());
            }, new object());

            Assert.AreEqual(2, result.MessageHandlerCount);
        }

        private static void Fail(object message) =>
            Assert.Fail($"Handler is not supposed to handle message of type '{message.GetType().FriendlyName()}'.");
    }
}
