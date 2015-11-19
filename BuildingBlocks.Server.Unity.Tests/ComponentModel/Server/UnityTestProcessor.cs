using Kingo.BuildingBlocks.Messaging;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    internal sealed class UnityTestProcessor : MessageProcessor
    {
        private readonly UnityFactory _messageHandlerFactory;

        internal UnityTestProcessor()
        {
            _messageHandlerFactory = new UnityFactory();
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _messageHandlerFactory; }
        }
    }
}
