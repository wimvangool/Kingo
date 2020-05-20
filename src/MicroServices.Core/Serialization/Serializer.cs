using System;
using System.Runtime.Serialization;
using Kingo.Reflection;

namespace Kingo.Serialization
{
    /// <summary>
    /// When implemented, represents a serializer that can serialize and deserialize
    /// objects using a specific serialization-protocol.
    /// </summary>
    public abstract class Serializer : ISerializer
    {
        #region [====== Serialize ======]

        /// <inheritdoc />
        public byte[] Serialize(object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            try
            {
                return SerializeContent(content);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewSerializationFailedException(content.GetType(), exception);
            }
        }

        /// <summary>
        /// Serializes the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">The content to serialize.</param>
        /// <returns>A byte-array representing the serialized object.</returns>
        protected abstract byte[] SerializeContent(object content);

        private static SerializationException NewSerializationFailedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.Serializer_SerializationFailed;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new SerializationException(message, exception);
        }

        #endregion

        #region [====== Deserialize ======]

        /// <inheritdoc />
        public object Deserialize(byte[] content, Type contentType)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (contentType == null)
            {
                throw new ArgumentNullException(nameof(contentType));
            }
            try
            {
                return DeserializeContent(content, contentType);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw NewDeserializationFailedException(contentType, exception);
            }
        }

        /// <summary>
        /// Deserializes the specified <paramref name="content"/> to an instance of type <paramref name="contentType"/>.
        /// </summary>
        /// <param name="content">A byte-array representing the serialized object.</param>
        /// <param name="contentType">Type of the target-object.</param>
        /// <returns>The deserialized instance of type <paramref name="contentType"/>.</returns>
        protected abstract object DeserializeContent(byte[] content, Type contentType);

        private static SerializationException NewDeserializationFailedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.Serializer_DeserializationFailed;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new SerializationException(message, exception);
        }

        #endregion
    }
}
