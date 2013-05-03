using System;

namespace YellowFlare.MessageProcessing
{        
    internal sealed class MessageCommand<TMessage> : IMessageCommand where TMessage : class
    {
        private readonly IMessageHandler<TMessage> _handler;
        private readonly TMessage _message;
        
        public MessageCommand(IMessageHandler<TMessage> handler, TMessage message)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _handler = handler;
            _message = message;
        }
        
        public object Handler
        {
            get { return _handler.Handler; }
        }
       
        public object Message
        {
            get { return _message; }
        }
        
        public void Execute()
        {
            _handler.Handle(_message);
        }
    }
}
