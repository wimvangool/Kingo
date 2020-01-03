using System;
using System.Runtime.Serialization;

namespace Kingo.Serialization
{
    /// <summary>
    /// Contains extensions methods for instances of type <see cref="ISerializer" />.
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Creates and returns a deep copy of the specified <paramref name="instance"/> by serializing and
        /// deserializing it with the specified <paramref name="serializer"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to copy.</typeparam>
        /// <param name="serializer">A serializer.</param>
        /// <param name="instance">The instance to copy.</param>
        /// <returns>A deep copy of the specified <paramref name="instance"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializer"/> or <paramref name="instance"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// Serialization or deserialization of the specified <paramref name="instance"/> failed.
        /// </exception>
        public static TValue Copy<TValue>(this ISerializer serializer, TValue instance) =>
            serializer.Deserialize<TValue>(NotNull(serializer).Serialize(instance));

        /// <summary>
        /// Deserializes and converts the specified <paramref name="value"/> to an instance of type <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of instance that the specified <paramref name="value"/> must be converted to.</typeparam>
        /// <param name="serializer">A serializer.</param>
        /// <param name="value">The value to deserialize.</param>
        /// <returns>
        /// An instance of type <typeparamref name="TValue"/> that represents the deserialized version
        /// of the specified <paramref name="value"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serializer"/> or <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// The specified <paramref name="value"/> could not be deserialized and/or converted to an instance of
        /// type <typeparamref name="TValue"/>.
        /// </exception>
        public static TValue Deserialize<TValue>(this ISerializer serializer, string value) =>
            (TValue) NotNull(serializer).Deserialize(value, typeof(TValue));

        private static ISerializer NotNull(ISerializer serializer) =>
            serializer ?? throw new ArgumentNullException(nameof(serializer));
    }
}
