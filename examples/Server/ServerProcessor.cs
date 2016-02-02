using System;
using System.Collections.Generic;
using Kingo.Messaging;
using Kingo.Samples.Chess.Players;
using Kingo.Transactions;

namespace Kingo.Samples.Chess
{
    public class ServerProcessor : MessageProcessor
    {
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
            yield return new FaultExceptionModule();
            yield return new MessageValidationModule();
            yield return new TransactionScopeModule();
        }

        #endregion

        #region [====== Main ======]

        private static void Main(string[] args)
        {
            using (var serviceHostManager = ServiceHostManager.CreateServiceHostManager())
            {
                Console.WriteLine("Starting all services...");

                serviceHostManager.Open();

                Console.WriteLine("All services have been started.");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();

                serviceHostManager.Close();

                Console.WriteLine("All services have been shut down.");
            }
        }                

        #endregion
    }
}
