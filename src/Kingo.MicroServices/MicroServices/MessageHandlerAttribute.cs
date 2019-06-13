using System;
using System.ComponentModel;

namespace Kingo.MicroServices
{
    /// <summary>
    /// This attribute can be declared on a message handler to configure its run-time behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class MessageHandlerAttribute : Attribute, IMessageHandlerConfiguration
    {     
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerAttribute" /> class.
        /// </summary>
        public MessageHandlerAttribute()
        {
            HandlesExternalMessages = true;
        }

        /// <inheritdoc />
        [DefaultValue(true)]
        public bool HandlesExternalMessages
        {
            get;
            set;
        }

        /// <inheritdoc />
        [DefaultValue(false)]
        public bool HandlesInternalMessages
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(HandlesExternalMessages)} = {HandlesExternalMessages}, {nameof(HandlesInternalMessages)} = {HandlesInternalMessages}";

        internal static MessageHandlerAttribute Create(bool handlesExternalMessages, bool handlesInternalMessages) =>
            new MessageHandlerAttribute() { HandlesExternalMessages = handlesExternalMessages, HandlesInternalMessages = handlesInternalMessages };        
    }
}
