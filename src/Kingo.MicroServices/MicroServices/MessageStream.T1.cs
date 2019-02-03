using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{    
    internal sealed class MessageStream<TMessage> : MessageStream
    {
        private readonly TMessage _message;        
        
        public MessageStream(TMessage message)
        {            
            _message = message;            
        }        

        #region [====== IReadOnlyList<object> ======]
        
        public override int Count =>
            1;
        
        public override IEnumerator<object> GetEnumerator()
        {
            yield return _message;
        }

        #endregion

        #region [====== HandleWithAsync ======]

        public override Task<MessageStream> HandleWithAsync(IMessageHandler handler) =>
            (handler ?? throw new ArgumentNullException(nameof(handler))).HandleAsync(_message);

        #endregion
    }
}
