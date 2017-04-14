using System.Threading.Tasks;

namespace Kingo.Messaging
{    
    internal abstract class EventStream : ReadOnlyList<object>, IMessageStream, IEventStream
    {                
        #region [====== IMessageStream ======]                         

        public IMessageStream Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null) =>
            AppendStream(MessageStream.CreateStream(message, handler));

        public abstract IMessageStream AppendStream(IMessageStream stream);

        public abstract Task HandleMessagesWithAsync(IMessageHandler handler);

        #endregion

        #region [====== IEventStream ======]

        public abstract void Publish<TEvent>(TEvent message);       

        #endregion
    }
}
