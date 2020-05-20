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
        #region [====== Clone ======]

        /// <summary>
        /// Clones the specified <paramref name="value"/> by serializing and deserializing it.
        /// </summary>
        /// <typeparam name="TValue">Type of the object to clone.</typeparam>
        /// <param name="value">Object to clone.</param>
        /// <returns>A deep clone of the object.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Serialization or deserialization of the object failed.
        /// </exception>
        TValue Clone<TValue>(TValue value) =>
            Deserialize<TValue>(Serialize(value));

        #endregion

        #region [====== Serialize ======]

        /// <summary>
        /// Serializes the specified <paramref name="content" />.
        /// </summary>
        /// <param name="content">The object to serialize.</param>
        /// <returns>A byte-array representing the serialized version of the specified <paramref name="content"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Serialization of the specified <paramref name="content"/> failed.
        /// </exception>
        byte[] Serialize(object content);

        #endregion

        #region [====== Deserialize ======]

        /// <summary>
        /// Deserializes the specified <paramref name="content"/> to an object of the specified type <typeparamref name="TValue" />.
        /// </summary>
        /// <typeparam name="TValue">The type of the object to which the value is deserialized.</typeparam>
        /// <param name="content">A byte-array representing the serialized version on an object.</param>
        /// <returns>An instance of the specified type <typeparamref name="TValue"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Deserialization of the specified <paramref name="content"/> to the specified <typeparamref name="TValue"/> failed.
        /// </exception>
        TValue Deserialize<TValue>(byte[] content) =>
            (TValue) Deserialize(content, typeof(TValue));

        /// <summary>
        /// Deserializes the specified <paramref name="content"/> to an object of the specified <paramref name="contentType"/>.
        /// </summary>
        /// <param name="content">A serialized object encoded as a string.</param>
        /// <param name="contentType">The type of the object to which the value is deserialized.</param>
        /// <returns>An instance of the specified <paramref name="contentType"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> or <paramref name="contentType"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Deserialization of the specified <paramref name="content"/> to the specified <paramref name="contentType"/> failed.
        /// </exception>
        object Deserialize(byte[] content, Type contentType);

        #endregion
    }
}
