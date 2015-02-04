using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Serves as a base class for all modules that base their behavior on some specific type of attribute declared on a message.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message to handle.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute to collect from a message.</typeparam>
    public abstract class SingleAttributeBasedModule<TMessage, TAttribute> : MessageAttributeBasedModule<TMessage, TAttribute>
        where TMessage : class
        where TAttribute : class
    {
        /// <summary>
        /// Selects a single attribute from <paramref name="attributes"/> and invokes
        /// <see cref="Handle(TMessage, TAttribute)"/> with the result.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <param name="attributes">All attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>.</param>
        protected override void Handle(TMessage message, IEnumerable<TAttribute> attributes)
        {
            Handle(message, attributes.SingleOrDefault());
        }

        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        /// <param name="attribute">
        /// The attribute that was declared on <paramref name="message"/>, or <c>null</c> if no attribute
        /// of type <typeparamref name="TAttribute"/> was declared.
        /// </param>
        protected abstract void Handle(TMessage message, TAttribute attribute);
    }
}
