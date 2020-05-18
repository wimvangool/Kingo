using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.Serialization
{
    /// <summary>
    /// When implemented, represents a <see cref="ISerializer" /> that is serializes objects to and from a string-format.
    /// </summary>
    public abstract class TextFormatSerializer : Serializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextFormatSerializer" /> class.
        /// </summary>
        /// <param name="encoder">Optional encoder that will be used by the serializer to convert string-values to and from byte-arrays.</param>
        protected TextFormatSerializer(ITextEncoder encoder = null)
        {
            Encoder = encoder ?? new TextEncoder(Encoding.UTF8);
        }

        /// <summary>
        /// Gets the encoder used to convert string-values to byte-arrays and vice versa.
        /// </summary>
        protected ITextEncoder Encoder
        {
            get;
        }

        #region [====== Serialize ======]

        /// <inheritdoc />
        protected override byte[] SerializeContent(object content) =>
            Encoder.Encode(SerializeToString(content));

        /// <summary>
        /// Serializes the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <returns>A string-version of the specified <paramref name="content"/>.</returns>
        protected abstract string SerializeToString(object content);

        #endregion

        #region [====== Deserialize ======]

        /// <inheritdoc />
        protected override object DeserializeContent(byte[] content, Type contentType) =>
            DeserializeFromString(Encoder.Decode(content), contentType);

        /// <summary>
        /// Deserializes the specified <paramref name="content"/> to an instance of type <paramref name="contentType"/>.
        /// </summary>
        /// <param name="content">A string-value representing the serialized object.</param>
        /// <param name="contentType">Type of the target-object.</param>
        /// <returns>The deserialized instance of type <paramref name="contentType"/>.</returns>
        protected abstract object DeserializeFromString(string content, Type contentType);

        #endregion
    }
}
