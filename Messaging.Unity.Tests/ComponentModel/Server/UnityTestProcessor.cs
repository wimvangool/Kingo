using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Server
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

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipelineModules()
        {
            return Enumerable.Empty<MessageHandlerModule>();
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
