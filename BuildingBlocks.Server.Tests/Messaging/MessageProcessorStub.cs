using Kingo.BuildingBlocks.ComponentModel.Server;

namespace Kingo.BuildingBlocks.Messaging
{
    internal sealed class MessageProcessorStub : MessageProcessor
    {
        private readonly UnityFactory _messageHandlerFactory;

        internal MessageProcessorStub()
        {
            _messageHandlerFactory = new UnityFactory();
        }

        protected internal override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }       
    }
}
