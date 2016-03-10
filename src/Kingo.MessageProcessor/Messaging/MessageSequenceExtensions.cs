using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Contains extension methods to types related to message sequences.
    /// </summary>
    public static class MessageSequenceExtensions
    {
        /// <summary>
        /// Creates and returns a concatened sequence of messages.
        /// </summary>
        /// <param name="sequences">The sequences to concatenate.</param>
        /// <returns>A concatened sequence of messages.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequences"/> is <c>null</c>.
        /// </exception>
        public static IMessageSequence Concatenate(this IEnumerable<IMessageSequence> sequences)
        {
            return new MessageSequenceConcatenation(sequences);
        }
    }
}
