using Kingo.Messaging;
using Kingo.Samples.Chess.Players;

namespace Kingo.Samples.Chess
{
    internal sealed class InMemoryProcessor : MessageProcessor
    {
        #region [====== MessageHandlerFactory ======]     

        protected override LayerConfiguration Customize(LayerConfiguration layers)
        {
            return layers.ReplaceDataAccessLayer(layers.ServiceLayer);
        }

        protected override MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            var factory = base.CreateMessageHandlerFactory(layers);
            factory.RegisterWithPerResolveLifetime(typeof(InMemoryPlayerRepository), typeof(IPlayerAdministration));
            return factory;
        }

        #endregion
    }
}
