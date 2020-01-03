using System;
using System.Text.Json;

namespace Kingo.Serialization
{
    /// <summary>
    /// Represents a <see cref="ISerializer"/> that uses JSON as a serialization format.
    /// </summary>
    public class JsonFormatSerializer : Serializer
    {
        #region [====== Serialize & Deserialize ======]

        /// <inheritdoc />
        public override string Serialize(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            try
            {
                return JsonSerializer.Serialize(instance, CreateSerializerOptions());
            }
            catch (Exception exception)
            {
                throw NewSerializationFailedException(instance.GetType(), exception);
            }
        }

        /// <inheritdoc />
        public override object Deserialize(string value, Type type)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            try
            {
                return JsonSerializer.Deserialize(value, type, CreateSerializerOptions());
            }
            catch (Exception exception)
            {
                throw NewDeserializationFailedException(type, exception);
            }
        }

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
