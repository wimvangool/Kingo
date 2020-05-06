using System;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a convention for resolving the message kind based on
    /// the type of a message.
    /// </summary>
    public interface IMessageKindResolver
    {
        /// <summary>
        /// Attempts to resolve the <see cref="MessageKind" /> of a message that has the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">Content of a message.</param>
        /// <returns>
        /// The <see cref="MessageKind"/> of the specified <paramref name="content" /> or <see cref="MessageKind.Undefined"/>
        /// if the kind could not be determined.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <c>null</c>.
        /// </exception>
        MessageKind ResolveMessageKind(object content) =>
            ResolveMessageKind(IsNotNull(content, nameof(content)).GetType());

        /// <summary>
        /// Attempts to resolve the <see cref="MessageKind" /> of a message that has the specified <paramref name="contentType"/>.
        /// </summary>
        /// <param name="contentType">Type of the message-content.</param>
        /// <returns>
        /// The <see cref="MessageKind"/> of the specified <paramref name="contentType" /> or <see cref="MessageKind.Undefined"/>
        /// if the kind could not be determined.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contentType"/> is <c>null</c>.
        /// </exception>
        MessageKind ResolveMessageKind(Type contentType);
    }
}
