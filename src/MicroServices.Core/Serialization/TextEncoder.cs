using System;
using System.Text;
using static Kingo.Ensure;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ITextEncoder"/> that uses an <see cref="Encoding"/> to encode and decode text.
    /// </summary>
    public sealed class TextEncoder : ITextEncoder
    {
        private readonly Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEncoder" /> class.
        /// </summary>
        /// <param name="encoding">The encoder to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public TextEncoder(Encoding encoding)
        {
            _encoding = IsNotNull(encoding, nameof(encoding));
        }

        /// <inheritdoc />
        public byte[] Encode(string content) =>
            _encoding.GetBytes(IsNotNull(content, nameof(content)));

        /// <inheritdoc />
        public string Decode(byte[] content) =>
            _encoding.GetString(IsNotNull(content, nameof(content)));
    }
}
