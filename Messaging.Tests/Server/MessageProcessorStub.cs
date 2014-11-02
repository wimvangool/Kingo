namespace System.ComponentModel.Server
{
    internal sealed class MessageProcessorStub : MessageProcessor
    {
        private readonly MessageHandlerFactory _factory;

        public MessageProcessorStub()
        {
            _factory = new MessageHandlerFactoryForUnity();
        }

        protected override MessageHandlerFactory MessageHandlerFactory
        {
            get { return _factory; }
        }
    }
}
