using System.Threading.Tasks;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerInstance<TMessage> : MessageHandlerInstance, IMessageHandler<TMessage> where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly Type _classType;
        private readonly Type _interfaceType;

        internal MessageHandlerInstance(IMessageHandler<TMessage> handler)
        {
            _handler = handler;
            _classType = handler.GetType();
            _interfaceType = typeof(IMessageHandler<TMessage>);
        }

        internal MessageHandlerInstance(IMessageHandler<TMessage> handler, Type classType, Type interfaceType)
        {
            _handler = handler;
            _classType = classType;
            _interfaceType = interfaceType;
        }

        protected override Type ClassType
        {
            get { return _classType; }
        }

        protected override Type InterfaceType
        {
            get { return _interfaceType; }
        }

        public Task HandleAsync(TMessage message)
        {
            return _handler.HandleAsync(message);
        }        
    }
}
