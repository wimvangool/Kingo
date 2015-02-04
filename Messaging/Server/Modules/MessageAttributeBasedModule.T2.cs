using System.Collections.Generic;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Serves as a base class for all modules that base their behavior on some specific type of attribute declared on a message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute(s) to collect from a message.</typeparam>
    public abstract class MessageAttributeBasedModule<TMessage, TAttribute> : MessageHandlerPipelineModule<TMessage>
        where TMessage : class
        where TAttribute : class
    {
        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>
        /// and invokes <see cref="Handle(TMessage, IEnumerable{TAttribute}" />.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected override void Handle(TMessage message)
        {
            Handle(message, MessageProcessor.SelectAttributesOfType<TAttribute>(message.GetType()));
        }

        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="attributes">All attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>.</param>
        protected abstract void Handle(TMessage message, IEnumerable<TAttribute> attributes);        
    }
}
