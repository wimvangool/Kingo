using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Modules;

namespace Kingo.ChessApplication
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public abstract class ChessApplicationService : MessageProcessor
    {
        private static readonly Lazy<UnityFactory> _MessageHandlerFactory = new Lazy<UnityFactory>(CreateMessageHandlerFactory, true);

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _MessageHandlerFactory.Value; }
        }

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            yield return new MessageValidationModule();
            yield return new TransactionScopeModule();
        }

        private static UnityFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers(BusinessLogicLayer());
            factory.RegisterRepositories(BusinessLogicLayer() + DataAccessLayer());
            return factory;
        }        

        private static AssemblySet BusinessLogicLayer()
        {
            return
                AssemblySet.FromCurrentDirectory("*.MessageHandlers.dll") +
                AssemblySet.FromCurrentDirectory("*.Domain.dll");
        }

        private static AssemblySet DataAccessLayer()
        {
            return AssemblySet.FromCurrentDirectory("*.Sql.dll");
        }
    }
}