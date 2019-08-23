using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a convention for resolving the message kind based on
    /// the type of a message.
    /// </summary>
    public interface IMessageKindResolver
    {
        /// <summary>
        /// Determines whether the message of the specified <paramref name="messageType"/> is a
        /// <see cref="MessageKind.Command"/> or an <see cref="MessageKind.Event"/>.
        /// </summary>
        /// <param name="messageType">Type of a message.</param>
        /// <returns>
        /// <see cref="MessageKind.Command"/> if the message is a command;
        /// <see cref="MessageKind.Event"/> if the message is an event.
        /// </returns>
        MessageKind ResolveMessageKind(Type messageType);
    }
}
