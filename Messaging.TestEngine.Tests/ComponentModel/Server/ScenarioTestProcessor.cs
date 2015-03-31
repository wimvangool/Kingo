using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Server
{
    internal sealed class ScenarioTestProcessor : MessageProcessor
    {        
        private readonly MessageHandlerFactory _messageHandlerFactory;

        private ScenarioTestProcessor(MessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }

        protected override IEnumerable<MessageHandlerModule> CreatePrimaryPipelineModules()
        {
            return Enumerable.Empty<MessageHandlerModule>();
        }

        private static readonly Lazy<ScenarioTestProcessor> _Instance = new Lazy<ScenarioTestProcessor>(CreateProcessor, true);

        public static ScenarioTestProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static ScenarioTestProcessor CreateProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity()
            {                
                ApplicationLayer = AssemblySet.CurrentAssembly(),
                DomainLayer = AssemblySet.CurrentAssembly(),
                DataAccessLayer = AssemblySet.CurrentAssembly()
            };
            messageHandlerFactory.RegisterMessageHandlers();

            return new ScenarioTestProcessor(messageHandlerFactory);
        }
    }
}
