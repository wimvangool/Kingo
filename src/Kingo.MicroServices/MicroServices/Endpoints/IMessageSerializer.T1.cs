using System;
using System.Collections.Generic;
using System.Text;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a message serializer that is able to serialize and pack
    /// messages into a message envelope, or deserialize and unpack message from such an envelope.
    /// </summary>
    /// <typeparam name="TMessageEnvelope">Type of the envelope.</typeparam>
    public interface IMessageSerializer<TMessageEnvelope>
    {
        /// <summary>
        /// Serializes the specified <paramref name="message"/> and packs it into an envelope.
        /// </summary>
        /// <param name="message">The message to pack and serialize.</param>
        /// <returns>The message envelope containing the contents of the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        TMessageEnvelope Serialize(object message);

        /// <summary>
        /// Unpacks and deserializes the specified <paramref name="message"/> to return its contents.
        /// </summary>
        /// <param name="message">The message to unpack and deserialize.</param>
        /// <returns>The contents of the specified <paramref name="message"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        object Deserialize(TMessageEnvelope message);
    }
}
