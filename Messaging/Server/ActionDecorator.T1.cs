namespace System.ComponentModel.Server
{
    internal sealed class ActionDecorator<TMessage> : IMessageHandler<TMessage> where TMessage : class
    {
        private readonly Action<TMessage> _handler;

        internal ActionDecorator(Action<TMessage> handler)
        {
            _handler = handler;
        }

        public void Handle(TMessage message)
        {
            _handler.Invoke(message);
        }
    }
}
