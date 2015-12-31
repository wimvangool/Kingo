using System.Threading.Tasks;
using Kingo.Messaging;
using NServiceBus;

namespace Kingo.Samples.Chess
{
    internal abstract class ServerProcessor : MessageProcessor
    {
        #region [====== BridgeBus ======]

        private sealed class BridgeBus : MessageProcessorBusDecorator
        {
            private readonly ServerProcessor _processor;
            private readonly IMessageProcessorBus _bus;

            internal BridgeBus(ServerProcessor processor, IMessageProcessorBus bus)
            {
                _processor = processor;
                _bus = bus;
            }

            protected override IMessageProcessorBus Bus
            {
                get { return _bus; }
            }

            public override async Task PublishAsync<TMessage>(TMessage message)
            {
                await Bus.PublishAsync(message);

                _processor.EnterpriseServiceBus.Publish(message);
            }
        }

        #endregion

        private readonly BridgeBus _bridgeBus;

        protected ServerProcessor()
        {
            _bridgeBus = new BridgeBus(this, base.EventBus);
        }

        public override IMessageProcessorBus EventBus
        {
            get { return _bridgeBus; }
        }

        protected abstract IBus EnterpriseServiceBus
        {
            get;
        }

        #region [====== MessageHandlerFactory ======]

        protected override MessageHandlerFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers("*.Application.dll", "*.DataAccess.dll");
            factory.RegisterRepositories("*.Domain.dll", "*.DataAccess.dll");
            return factory;
        }

        #endregion   
    }
}
