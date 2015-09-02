using System;
using System.Collections.Generic;
using System.Reflection;
using Kingo.BuildingBlocks;
using Kingo.BuildingBlocks.Clocks;
using Kingo.BuildingBlocks.ComponentModel.Server;
using Kingo.BuildingBlocks.Messaging;
using Kingo.BuildingBlocks.Messaging.Modules;

namespace Kingo.ChessApplication
{
    /// <summary>
    /// Represents a <see cref="MessageProcessor" />.
    /// </summary>
    public sealed class UnitTestProcessor : MessageProcessor
    {
        private static readonly Lazy<UnityFactory> _MessageHandlerFactory = new Lazy<UnityFactory>(CreateMessageHandlerFactory, true);

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _MessageHandlerFactory.Value; }
        }

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipeline()
        {
            yield return new ClockModule(HighResolutionClock.Default);
        }

        private static UnityFactory CreateMessageHandlerFactory()
        {
            var factory = new UnityFactory();
            factory.RegisterMessageHandlers(BusinessLogicLayer());
            factory.RegisterRepositories(BusinessLogicLayer() + UnitTestLayer());
            return factory;
        }

        private static AssemblySet UnitTestLayer()
        {
            return Assembly.GetExecutingAssembly();
        }

        private static AssemblySet BusinessLogicLayer()
        {
            return
                AssemblySet.FromCurrentDirectory("*.MessageHandlers.dll") +
                AssemblySet.FromCurrentDirectory("*.Domain.dll");
        }        
    }
}