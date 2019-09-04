using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to the <see cref="IMessageHandler{TMessage}.HandleAsync"/> method implementation
    /// of a message handler, indicates that that method serves as an endpoint and will be returned as such
    /// by the microprocessor's <see cref="IMicroServiceBusProcessor.CreateServiceBusEndpoints"/> method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EndpointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointAttribute" /> class.
        /// </summary>
        /// <param name="messageKind">Specifies the message kind of the message that is handled by this endpoint.</param>
        public EndpointAttribute(MessageKind messageKind = MessageKind.Unspecified)
        {
            MessageKind = messageKind;
        }

        #region [====== MessageKind ======]

        /// <summary>
        /// Specifies the message kind of the message that is handled by this endpoint
        /// </summary>
        public MessageKind MessageKind
        {
            get;
        }

        internal bool IsCommandEndpoint(IMessageKindResolver resolver, Type messageType) =>
            IsCommandEndpoint(resolver, messageType, MessageKind);

        private static bool IsCommandEndpoint(IMessageKindResolver resolver, Type messageType, MessageKind messageKind)
        {
            switch (messageKind)
            {
                case MessageKind.Unspecified:
                    return IsCommandEndpoint(new MessageKindResolver(), messageType, resolver.ResolveMessageKind(messageType));
                case MessageKind.Command:
                    return true;
                case MessageKind.Event:
                    return false;
            }
            throw NewInvalidMessageKindSpecifiedException(messageKind);
        }

        private static Exception NewInvalidMessageKindSpecifiedException(MessageKind messageKind)
        {
            var messageFormat = ExceptionMessages.EndpointAttribute_InvalidMessageKindSpecified;
            var message = string.Format(messageFormat, messageKind);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
