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
        /// <inheritdoc />
        public abstract string Serialize(object instance);

        /// <inheritdoc />
        public abstract object Deserialize(string value, Type type);

        /// <summary>
        /// Creates and returns an exception that indicates serialization of an object of type
        /// <paramref name="type"/> failed.
        /// </summary>
        /// <param name="type">Type of the instance that was being serialized.</param>
        /// <param name="exception">Exception that was thrown while serializing the object.</param>
        /// <returns>A wrapper exception indicating the error.</returns>
        protected static SerializationException NewSerializationFailedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.Serializer_SerializationFailed;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new SerializationException(message, exception);
        }

        /// <summary>
        /// Creates and returns an exception that indicates deserialization of an object of type
        /// <paramref name="type"/> failed.
        /// </summary>
        /// <param name="type">Type of the instance that was being deserialized.</param>
        /// <param name="exception">Exception that was thrown while deserializing the object.</param>
        /// <returns>A wrapper exception indicating the error.</returns>
        protected static SerializationException NewDeserializationFailedException(Type type, Exception exception)
        {
            var messageFormat = ExceptionMessages.Serializer_DeserializationFailed;
            var message = string.Format(messageFormat, type.FriendlyName());
            return new SerializationException(message, exception);
        }
    }
}
