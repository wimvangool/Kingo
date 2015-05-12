using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Server
{
    internal sealed class MessageProcessorStub : MessageProcessor
    {
        private readonly MessageHandlerFactory _factory;

        public MessageProcessorStub()
        {
            _factory = new MessageHandlerFactoryForUnity();
        }

        protected internal override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _factory; }
        }

        protected override IEnumerable<MessageHandlerModule> CreateMessageEntryPipelineModules()
        {
            return Enumerable.Empty<MessageHandlerModule>();
        }
    }
}
