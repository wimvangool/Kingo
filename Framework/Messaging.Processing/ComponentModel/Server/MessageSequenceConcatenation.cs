using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Server
{
    internal sealed class MessageSequenceConcatenation : MessageSequence
    {
        private readonly IEnumerable<IMessageSequence> _messages;

        internal MessageSequenceConcatenation(IEnumerable<IMessageSequence> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }
            _messages = messages;
        }

        public override async Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            foreach (var sequence in _messages.Where(sequence => sequence != null))
            {
                await sequence.ProcessWithAsync(processor, token);
            }
        }
    }
}
