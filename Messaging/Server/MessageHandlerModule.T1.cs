using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all modules that base their behavior on some specific type of attribute declared on a message.
    /// </summary>    
    /// <typeparam name="TAttribute">Type of the attribute(s) to collect from a message.</typeparam>
    public abstract class MessageHandlerModule<TAttribute> : MessageHandlerModule where TAttribute : Attribute
    {
        /// <inheritdoc />
        protected override void InvokeHandler(IMessageHandler handler)
        {            
            InvokeHandler(handler, handler.Message.SelectAttributesOfType<TAttribute>());
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> while adding specific pipeline logic.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <param name="attributes">All attributes of type <typeparamref name="TAttribute"/> declared on the incoming message.</param>
        protected abstract void InvokeHandler(IMessageHandler handler, IEnumerable<TAttribute> attributes);        
    }
}
