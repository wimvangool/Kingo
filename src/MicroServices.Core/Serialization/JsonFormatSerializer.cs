using System;
using System.Text.Json;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ISerializer"/> that uses JSON as a serialization format.
    /// </summary>
    public class JsonFormatSerializer : TextFormatSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFormatSerializer" /> class.
        /// </summary>
        /// <param name="encoder">Optional encoder that will be used by the serializer to convert string-values to and from byte-arrays.</param>
        public JsonFormatSerializer(ITextEncoder encoder = null) :
            base(encoder) { }

        #region [====== Serialize ======]

        /// <inheritdoc />
        protected override string SerializeToString(object content) =>
            JsonSerializer.Serialize(content, CreateSerializerOptions());

        /// <inheritdoc />
        protected override object DeserializeFromString(string content, Type contentType) =>
            JsonSerializer.Deserialize(content, contentType, CreateSerializerOptions());

        /// <summary>
        /// Creates and returns the options that determine how the serializer should serialize
        /// and deserialize objects.
        /// </summary>
        /// <returns>A new options-object.</returns>
        protected virtual JsonSerializerOptions CreateSerializerOptions() =>
            new JsonSerializerOptions();

        #endregion
    }
}
