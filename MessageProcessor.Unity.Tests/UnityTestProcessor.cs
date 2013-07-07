using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class UnityTestProcessor : IMessageProcessor
    {
        private readonly MessageProcessor _core;

        private UnityTestProcessor(MessageHandlerFactory handlerFactory)
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

        private static readonly Lazy<UnityTestProcessor> _Instance = new Lazy<UnityTestProcessor>(CreateProcessor, true);

        public static UnityTestProcessor Instance
        {
            get { return _Instance.Value; }
        }

        private static UnityTestProcessor CreateProcessor()
        {
            return new UnityTestProcessor(new MessageHandlerFactoryForUnity());
        }
    }
}
