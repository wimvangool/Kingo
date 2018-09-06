using System;
using System.Threading.Tasks;
using static Kingo.Threading.AsyncMethod;

namespace Kingo.Messaging
{
    internal sealed class EmptyMessageStream : EmptyList<object>, IMessageStream
    {
        #region [====== IMessageStream ======]  

        public override string ToString() =>
            string.Empty;        

        public IMessageStream Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler = null) =>
            MessageStream.CreateStream(message, handler);

        public IMessageStream AppendStream(IMessageStream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            return stream;
        }

        public Task HandleMessagesWithAsync(IMessageHandler handler) =>
            NoValue;               

        #endregion
    }
}
