using System;

namespace YellowFlare.MessageProcessing
{
    internal sealed class UnityTestProcessor : MessageProcessor
    {        
        private UnityTestProcessor(MessageHandlerFactory handlerFactory) : base(handlerFactory) { }        

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
