using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    /// <summary>
    /// Represents a pair of message sequences put together as a single sequence.
    /// </summary>
    public class MessageSequencePair : MessageSequence
    {
        private readonly IMessageSequence _left;
        private readonly IMessageSequence _right;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSequencePair" /> class.
        /// </summary>
        /// <param name="left">The first sequence.</param>
        /// <param name="right">The second sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
        /// </exception>
        public MessageSequencePair(IMessageSequence left, IMessageSequence right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            _left = left;
            _right = right;
        }

        /// <inheritdoc />
        public override async Task ProcessWithAsync(IMessageProcessor processor, CancellationToken token)
        {
            await _left.ProcessWithAsync(processor, token);
            await _right.ProcessWithAsync(processor, token);
        }
    }
}
