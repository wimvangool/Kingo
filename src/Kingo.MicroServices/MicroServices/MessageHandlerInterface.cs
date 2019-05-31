using System;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a specific, closed version of the <see cref="IMessageHandler{TMessage}"/> interface.
    /// </summary>
    public sealed class MessageHandlerInterface : IEquatable<MessageHandlerInterface>
    {
        internal MessageHandlerInterface(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        /// <summary>
        /// The type of the interface.
        /// </summary>
        public Type InterfaceType
        {
            get;
        }

        /// <summary>
        /// The message type of the message that is handled by the <see cref="IMessageHandler{TMessage}"/>.
        /// </summary>
        public Type MessageType =>
            InterfaceType.GetGenericArguments()[0];

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            Equals(obj as MessageHandlerInterface);

        /// <inheritdoc />
        public bool Equals(MessageHandlerInterface other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return InterfaceType == other.InterfaceType;
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            InterfaceType.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            InterfaceType.FriendlyName();
    }
}
