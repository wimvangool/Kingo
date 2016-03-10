using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class MessageSequenceConcatenation : MessageSequence
    {
        private readonly IEnumerable<IMessageSequence> _sequences;

        internal MessageSequenceConcatenation(IEnumerable<IMessageSequence> sequences)
        {
            if (sequences == null)
            {
                throw new ArgumentNullException(nameof(sequences));
            }
            _sequences = sequences;
        }

        public override async Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            foreach (var sequence in _sequences.Where(sequence => sequence != null))
            {
                await sequence.ProcessWithAsync(processor, token);
            }
        }
    }
}
