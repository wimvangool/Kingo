using System;
using System.Runtime.Serialization;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// When implemented by a class, represents a component that is able to serialize and deserialize
    /// <see cref="IMessage"/>-objects.
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serializes the specified <paramref name="message"/> into a version that can be transmitted over a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <param name="message">The message to serialize.</param>
        /// <returns>A serialized version of the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// <paramref name="message"/> could not be serialized.
        /// </exception>
        IMicroServiceBusMessage Serialize(IMessage message);

        /// <summary>
        /// Deserializes the specified <paramref name="message"/> into a version that can be processed by a <see cref="IMicroProcessor"/>.
        /// </summary>
        /// <param name="message">The message to deserialize.</param>
        /// <param name="kind">The (expected) kind of the message.</param>
        /// <returns>A deserialized version of the specified <paramref name="message"/> that will serve as input for a processor.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// <paramref name="message"/> could not be deserialized.
        /// </exception>
        IMessage DeserializeInput(IMicroServiceBusMessage message, MessageKind kind);

        /// <summary>
        /// Deserializes the specified <paramref name="message"/> into a version that can be forwarded to another <see cref="IMicroServiceBus"/>.
        /// </summary>
        /// <param name="message">The message to deserialize.</param>
        /// <param name="kind">The (expected) kind of the message.</param>
        /// <returns>A deserialized version of the specified <paramref name="message"/> that can be forwarded to another <see cref="IMicroServiceBus"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="SerializationException">
        /// <paramref name="message"/> could not be deserialized.
        /// </exception>
        IMessage DeserializeOutput(IMicroServiceBusMessage message, MessageKind kind);
    }
}
