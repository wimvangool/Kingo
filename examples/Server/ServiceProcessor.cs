using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players;
using Kingo.Transactions;

namespace Kingo.Samples.Chess
{
    public class ServiceProcessor : MessageProcessor
    {
        protected override async Task PublishAsync<TEvent>(TEvent @event)
        {
            await base.PublishAsync(@event);

            var serviceBus = ServiceBus.Instance;
            if (serviceBus != null)
            {
                serviceBus.Publish(@event);
            }
        }

        #region [====== MessageHandlerFactory ======]

        protected override MessageHandlerFactory CreateMessageHandlerFactory(LayerConfiguration layers)
        {
            var factory = base.CreateMessageHandlerFactory(layers);
            factory.RegisterWithPerResolveLifetime(typeof(PlayersTable), typeof(IPlayerAdministration));
            return factory;
        }

        #endregion

        #region [====== Pipeline Configuration ======]

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            yield return new ErrorHandlerModule();
            yield return new MessageValidationModule();
            yield return new TransactionScopeModule();
        }

        #endregion        
    }
}
