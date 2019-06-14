using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When applied to the <see cref="IMessageHandler{TMessage}.HandleAsync"/> method implementation
    /// of a message handler, indicates that that method serves as an endpoint and will be returned as such
    /// by the microprocessor's <see cref="IMicroProcessor.CreateMethodEndpoints"/> method.
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

        /// <summary>
        /// Specifies the message kind of the message that is handled by this endpoint
        /// </summary>
        public MessageKind MessageKind
        {
            get;
        }        
    }
}
