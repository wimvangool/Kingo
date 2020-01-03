using System;
using System.Runtime.Serialization;

namespace Kingo.Serialization
{
    /// <summary>
    /// When implemented by a class, represents a serializer that can serialize and deserialize
    /// objects using a specific serialization-protocol.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the specified <paramref name="instance" />.
        /// </summary>
        /// <param name="instance">The object to serialize.</param>
        /// <returns>The serialized object encoded as a string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Serialization of the specified <paramref name="instance"/> failed.
        /// </exception>
        string Serialize(object instance);

        /// <summary>
        /// Deserializes the specified <paramref name="value"/> to an object of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="value">A serialized object encoded as a string.</param>
        /// <param name="type">The type of the object to which the value is deserialized.</param>
        /// <returns>An instance of the specified <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Deserialization of the specified <paramref name="value"/> to the specified <paramref name="type"/> failed.
        /// </exception>
        object Deserialize(string value, Type type);
    }
}
