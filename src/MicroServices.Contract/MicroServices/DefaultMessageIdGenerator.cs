using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IMessageIdGenerator"/> interface by
    /// generating unique identifiers for specific messages.
    /// </summary>
    public sealed class DefaultMessageIdGenerator : IMessageIdGenerator
    {
        /// <inheritdoc />
        public string GenerateMessageId(object content) =>
            GenerateGuid(IsNotNull(content, nameof(content))).ToString();

        private static Guid GenerateGuid(object content) =>
            new Guid(GenerateGuidBytes(content).SelectMany(bytes => bytes).ToArray());

        private static IEnumerable<IEnumerable<byte>> GenerateGuidBytes(object content)
        {
            yield return BitConverter.GetBytes(content.GetType().GetHashCode());
            yield return BitConverter.GetBytes(content.GetHashCode());
            yield return Guid.NewGuid().ToByteArray().Skip(8);
        }
    }
}
