using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Messaging;

namespace Kingo.Samples.Chess
{
    public sealed class SecureMessage<TMessage> : MessageToHandle<TMessage> where TMessage : class, IMessage
    {
        private readonly Session _session;

        public SecureMessage(TMessage message, Guid playerId, string playerName)
            : this(message, new Session(playerId, playerName)) { }

        public SecureMessage(TMessage message, Session session)
            : base(message)
        {
            _session = session;
        }

        public override async Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            using (Session.CreateSessionScope(_session))
            {
                await base.ProcessWithAsync(processor, token);
            }
        }
    }
}
