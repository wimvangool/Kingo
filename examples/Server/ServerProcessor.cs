using Kingo.Messaging;
using Kingo.Samples.Chess.Players;
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

        protected override MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            var factory = base.CreateMessageHandlerFactory(layers);
            factory.RegisterWithPerResolveLifetime(typeof(PlayersTable), typeof(IPlayerAdministration));
            return factory;
        }

        #endregion   
    }
}
