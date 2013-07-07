using System;
using System.Reflection;

namespace YellowFlare.MessageProcessing
{
    internal sealed class ScenarioTestProcessor : IMessageProcessor
    {
        private readonly MessageProcessor _core;

        private ScenarioTestProcessor(MessageHandlerFactory handlerFactory)
        {
            _core = new MessageProcessor(this, handlerFactory);
        }

        public IMessageProcessorBus Bus
        {
            get { return _core.Bus; }
        }

        public void Handle<TMessage>(TMessage message) where TMessage : class
        {
            _core.Handle(message);
        }

        public void Handle<TMessage>(TMessage message, IMessageHandler<TMessage> handler) where TMessage : class
        {
            _core.Handle(message, handler);
        }

        public void Handle<TMessage>(TMessage message, Action<TMessage> action) where TMessage : class
        {
            _core.Handle(message, action);
        }

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
