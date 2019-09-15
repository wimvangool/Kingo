using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to an implementation of the <see cref="IMessageHandler{TMessage}.HandleAsync"/>
    /// method, indicates that the message-handler will receive commands or events from the service bus.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MicroServiceBusEndpointAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusEndpointAttribute" /> class.
        /// </summary>
        public MicroServiceBusEndpointAttribute() :
            this(MessageKind.Unspecified) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroServiceBusEndpointAttribute" /> class.
        /// </summary>
        /// <param name="messageKind">If specified, indicates what kind of message is handled by this endpoint.</param>
        public MicroServiceBusEndpointAttribute(MessageKind messageKind)
        {
            MessageKind = messageKind;
        }

        /// <summary>
        /// Gets or sets the message kind of the message that is handled by this endpoint.
        /// </summary>
        public MessageKind MessageKind
        {
            get;
        }

        internal MessageKind DetermineMessageKind(IMessageKindResolver resolver, Type messageType)
        {
            switch (MessageKind)
            {
                case MessageKind.Unspecified:
                    return resolver.ResolveMessageKind(messageType);
                case MessageKind.Command:
                    return MessageKind.Command;
                case MessageKind.Event:
                    return MessageKind.Event;
            }
            throw NewInvalidMessageKindSpecifiedException(MessageKind);
        }

        internal static Exception NewInvalidMessageKindSpecifiedException(MessageKind messageKind)
        {
            var messageFormat = ExceptionMessages.EndpointAttribute_InvalidMessageKindSpecified;
            var message = string.Format(messageFormat, messageKind);
            return new InvalidOperationException(message);
        }
    }
}
