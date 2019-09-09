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
        /// <param name="name">Name of this endpoint. </param>
        public MicroServiceBusEndpointAttribute(string name = null)
        {
            Name = name;
        }

        /// <summary>
        /// Name of this endpoint.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets the message kind of the message that is handled by this endpoint.
        /// </summary>
        public MessageKind MessageKind
        {
            get;
            set;
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
    }
}
