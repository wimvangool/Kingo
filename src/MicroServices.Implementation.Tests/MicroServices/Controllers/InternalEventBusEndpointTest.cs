using System;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Controllers
{
    [TestClass]
    public sealed class InternalEventBusEndpointTest : MicroProcessorTest<MicroProcessor>
    {
        [TestMethod]
        public async Task HandleAsync_DoesNotHandleEvent_IfEventIsScheduled_And_EndpointDoesNotAcceptScheduledEvents()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<object>((message, context) =>
            {
                Assert.Fail($"Handler is not supposed to handle event {message.GetType().FriendlyName()}.");
            });

            var result = await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.MessageBus.PublishEvent(new object(), DateTimeOffset.Now);
            }, new object());

            Assert.AreEqual(1, result.MessageHandlerCount);
        }

        [TestMethod]
        public async Task HandleAsync_HandlesEvent_IfEventIsNotScheduled_And_EndpointDoesNotAcceptScheduledEvents()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<object>((message, context) =>
            {
                Assert.AreEqual(2, context.StackTrace.Count);
            });

            var result = await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.MessageBus.PublishEvent(new object());
            }, new object());

            Assert.AreEqual(2, result.MessageHandlerCount);
        }

        [TestMethod]
        public async Task HandleAsync_HandlesEvent_IfEventIsScheduled_And_EndpointAcceptsScheduledEvents()
        {
            ProcessorBuilder.MessageHandlers.AddInstance<object>((message, context) =>
            {
                Assert.AreEqual(2, context.StackTrace.Count);
            }, true);

            var result = await CreateProcessor().ExecuteCommandAsync((message, context) =>
            {
                context.MessageBus.PublishEvent(new object(), DateTimeOffset.Now);
            }, new object());

            Assert.AreEqual(2, result.MessageHandlerCount);
        }
    }
}
