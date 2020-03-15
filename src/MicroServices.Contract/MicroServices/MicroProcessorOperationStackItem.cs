using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents one item/operation of a <see cref="MicroProcessorOperationStackTrace" />.
    /// </summary>
    [Serializable]
    public sealed class MicroProcessorOperationStackItem 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorOperationStackItem" /> class.
        /// </summary>
        /// <param name="componentType">Type of the message handler or query that was processing the specified <paramref name="message"/>.</param>
        /// <param name="message">The message that was being processed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="componentType"/> is <c>null</c>.
        /// </exception>
        public MicroProcessorOperationStackItem(Type componentType, IMessageEnvelope message = null)
        {
            ComponentType = componentType ?? throw new ArgumentNullException(nameof(componentType));
            Message = message == null ? null : new MessageEnvelope(message);
        }

        /// <summary>
        /// Type of the message handler or query that was processing the specified <see name="Message"/>.
        /// </summary>
        public Type ComponentType
        {
            get;
        }

        /// <summary>
        /// The message that was being processed.
        /// </summary>
        public MessageEnvelope Message
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            ToString(ComponentType.FriendlyName());

        private string ToString(string componentType) =>
            Message == null ? componentType : $"{componentType}({Message.Content.GetType().FriendlyName()})";
    }
}
