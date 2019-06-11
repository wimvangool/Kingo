using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    internal sealed class Message<TMessage> : MessageType, IMessage<TMessage>
    {        
        private readonly TMessage _message;                
        
        public Message(TMessage message, MessageKind kind) :
            base(typeof(TMessage), kind)
        {                       
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }            
            _message = message;            
        }

        #region [====== IMessage<TMessage> ======]        

        object IMessage.Instance =>
            Instance;
        
        public TMessage Instance =>
            _message;        

        #endregion              
    }
}
