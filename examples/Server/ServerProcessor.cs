using Kingo.Messaging;
using NServiceBus;

namespace Kingo.Samples.Chess
{
    internal abstract class ServerProcessor : MessageProcessor
    {
        protected abstract IBus EnterpriseServiceBus
        {
            get;
        }

        protected override void OnPublishing<TEvent>(TEvent @event)
        {
            base.OnPublishing(@event);

            EnterpriseServiceBus.Publish(@event);
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
