using Kingo.Messaging;
using Kingo.Samples.Chess.Players;
using Microsoft.Practices.Unity;

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
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers(layers);
            factory.RegisterRepositories(layers);
            factory.Container.RegisterType(typeof(IPlayerAdministration), typeof(InMemoryPlayerRepository));
            return factory;
        }

        #endregion
    }
}
