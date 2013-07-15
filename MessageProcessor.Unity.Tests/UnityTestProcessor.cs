using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class UnityTestProcessor : MessageProcessor
    {        
        private readonly MessageHandlerFactory _messageHandlerFactory;

        private UnityTestProcessor(MessageHandlerFactory messageHandlerFactory)
        {
            _messageHandlerFactory = messageHandlerFactory;
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
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
