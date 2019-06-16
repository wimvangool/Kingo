using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Endpoints
{
    [TestClass]
    public sealed class MicroServiceBusEndpointTest : MicroProcessorTest<MicroProcessor>
    {
        #region [====== Controller & Endpoint ======]

        private sealed class MicroServiceBusControllerStub : MicroServiceBusController
        {
            private readonly IMicroProcessor _processor;
            private readonly IMicroServiceBus _serviceBus;

            public MicroServiceBusControllerStub(IMicroProcessor processor, IMicroServiceBus serviceBus)
            {
                _processor = processor;
                _serviceBus = serviceBus;
            }

            protected override IMicroProcessor Processor =>
                _processor;

            protected override HostedEndpoint CreateHostedEndpoint(HandleAsyncMethodEndpoint methodEndpoint) =>
                new MicroServiceBusEndpointStub(methodEndpoint, _serviceBus);
        }

        private sealed class MicroServiceBusEndpointStub : MicroServiceBusEndpoint<int>
        {
            private readonly HandleAsyncMethodEndpoint _endpoint;
            private readonly IMicroServiceBus _serviceBus;

            public MicroServiceBusEndpointStub(HandleAsyncMethodEndpoint endpoint, IMicroServiceBus serviceBus)
            {
                _endpoint = endpoint;
                _serviceBus = serviceBus;
            }

            protected override Task ConnectAsync(CancellationToken cancellationToken) =>
                HandleAsync(MessageHandlerStub.ExpectedMessage);

            protected override Task DisconnectAsync(CancellationToken cancellationToken) =>
                Task.CompletedTask;

            protected override HandleAsyncMethodEndpoint MethodEndpoint =>
                _endpoint;

            protected override IMicroServiceBus ServiceBus =>
                _serviceBus;

            protected override object Deserialize(int message) =>
                message;
        }

        private sealed class MessageHandlerStub : IMessageHandler<object>, IMessageHandler<int>
        {
            public const int ExpectedMessage = 10;

            private readonly IInstanceCollector _instances;

            public MessageHandlerStub(IInstanceCollector instances)
            {
                _instances = instances ?? throw new ArgumentNullException(nameof(instances));
            }

            [Endpoint(MessageKind.Command)]
            Task IMessageHandler<object>.HandleAsync(object message, MessageHandlerOperationContext context) =>
                HandleAsync((int) message, context);

            [Endpoint(MessageKind.Event)]
            public Task HandleAsync(int message, MessageHandlerOperationContext context)
            {
                Assert.AreEqual(ExpectedMessage, message);
                context.EventBus.Publish(MicroServiceBusStub.ExpectedEvent);
                _instances.Add(this);
                return Task.CompletedTask;
            }                            
        }

        private sealed class MicroServiceBusStub : MicroServiceBus
        {
            public static readonly object ExpectedEvent = new object();

            private int _eventCount;

            public override Task PublishAsync(IMessage message)
            {
                Assert.AreSame(ExpectedEvent, message.Instance);
                _eventCount++;
                return Task.CompletedTask;
            }

            public void AssertEventCountIs(int count) =>
                Assert.AreEqual(count, _eventCount);
        }

        #endregion

        [TestMethod]
        public async Task HandleAsync_HandlesTheReceivedMessageAndPublishesTheResultingEvents()
        {
            ProcessorBuilder.Components.AddMessageHandler<MessageHandlerStub>();

            var processor = CreateProcessor();
            var serviceBus = new MicroServiceBusStub();
            var controller = new MicroServiceBusControllerStub(processor, serviceBus);
            var instances = processor.ServiceProvider.GetRequiredService<IInstanceCollector>();

            await controller.StartAsync(CancellationToken.None);

            instances.AssertInstanceCountIs(2);
            serviceBus.AssertEventCountIs(2);
        }
    }
}
