using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;
using static Kingo.Ensure;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Provides a basic implementation of the <see cref="IMessageKindResolver" /> interface that will
    /// resolve the <see cref="MessageKind" /> based on the message's type-name.
    /// </summary>
    public sealed class DefaultMessageKindResolver : IMessageKindResolver
    {
        private readonly StringComparison _comparisonType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageKindResolver" /> class.
        /// </summary>
        /// <param name="comparisonType">
        /// Determines how the name of the content-types are compared to the name of the
        /// <see cref="MessageKind" />.
        /// </param>
        public DefaultMessageKindResolver(StringComparison comparisonType = StringComparison.Ordinal)
        {
            _comparisonType = comparisonType;
        }

        /// <inheritdoc />
        public MessageKind ResolveMessageKind(Type contentType) =>
            ResolveMessageKind(IsNotNull(contentType, nameof(contentType)).FriendlyName(false, false));

        private MessageKind ResolveMessageKind(string contentTypeName) =>
            ResolveMessageKind(contentTypeName, MessageKind.Command, MessageKind.Event, MessageKind.Request, MessageKind.Response);

        private MessageKind ResolveMessageKind(string contentTypeName, params MessageKind[] knownKinds)
        {
            foreach (var messageKind in knownKinds)
            {
                if (contentTypeName.EndsWith(messageKind.ToString(), _comparisonType))
                {
                    return messageKind;
                }
            }
            return MessageKind.Undefined;
        }
    }
}
