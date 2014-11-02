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

        private static readonly Lazy<ScenarioTestProcessor> _Instance = new Lazy<ScenarioTestProcessor>(CreateProcessor, true);

        public static ScenarioTestProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static ScenarioTestProcessor CreateProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity();
            messageHandlerFactory.RegisterMessageHandlersFrom(Assembly.GetExecutingAssembly());
            return new ScenarioTestProcessor(messageHandlerFactory);
        }
    }
}
