using System;
using System.Threading;
using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    internal sealed class EmptyMessageSequence : MessageSequence
    {
        public override Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            return AsyncMethod.Void;
        }

        public override IMessageSequence Append(IMessageSequence sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException(nameof(sequence));
            }
            return sequence;
        }

        public override IMessageSequence Append<TMessage>(TMessage message, IMessageHandler<TMessage> handler)
        {
            return new MessageSequenceNode<TMessage>(message, handler);
        }
    }
}
