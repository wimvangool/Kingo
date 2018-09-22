using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal sealed class MessageStream<TMessage> : ReadOnlyList<object>, IMessageStream
    {
        private readonly TMessage _message;
        private readonly IMessageHandler<TMessage> _handler;                
        
        public MessageStream(TMessage message, IMessageHandler<TMessage> handler = null)
        {            
            _message = message;
            _handler = handler;
        }

        public override string ToString() =>
            _message.GetType().FriendlyName();

        #region [====== IReadOnlyList<object> ======]
        
        public override int Count =>
            1;
        
        public override IEnumerator<object> GetEnumerator()
        {
            yield return _message;
        }        

        #endregion

        #region [====== IMessageStream ======]                       
        
        public IMessageStream Append<TOther>(TOther message, IMessageHandler<TOther> handler = null) =>
            new MessageStream(this, MessageStream.CreateStream(message, handler));
        
        public IMessageStream AppendStream(IMessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (stream.Count == 0)
            {
                return this;
            }
            return new MessageStream(this, stream);
        }
        
        public Task HandleMessagesWithAsync(IMessageHandler handler) =>
            handler == null ? Task.CompletedTask : handler.HandleAsync(_message, _handler);

        #endregion
    }
}
