using System;
using System.Threading.Tasks;
using Kingo.Resources;
using Kingo.Threading;

namespace Kingo.Messaging
{
    internal sealed class EmptyMessageStream : EmptyList<object>, IMessageStream
    {
        #region [====== IMessageStream ======]  

        public override string ToString() =>
            $"<{DebugMessages.Empty}>";

        public IMessageStream Append<TMessage>(TMessage message, Action<TMessage, IMicroProcessorContext> handler) =>
            MessageStream.CreateStream(message, handler);

        public IMessageStream Append<TMessage>(TMessage message, Func<TMessage, IMicroProcessorContext, Task> handler) =>
            MessageStream.CreateStream(message, handler);

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
            AsyncMethod.Void;               

        #endregion
    }
}
