using System;
using System.Reflection;

namespace YellowFlare.MessageProcessing
{
    internal sealed class ScenarioTestProcessor : MessageProcessor
    {        
        private ScenarioTestProcessor(MessageHandlerFactory handlerFactory) : base(handlerFactory) { }

        private static readonly Lazy<ScenarioTestProcessor> _Instance = new Lazy<ScenarioTestProcessor>(CreateProcessor, true);

        public static ScenarioTestProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static ScenarioTestProcessor CreateProcessor()
        {
            var messageHandlerFactory = new MessageHandlerFactoryForUnity();
            messageHandlerFactory.RegisterMessageHandlers(Assembly.GetExecutingAssembly());
            return new ScenarioTestProcessor(messageHandlerFactory);
        }
    }
}
