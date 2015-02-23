using System.Collections.Generic;
using System.Linq;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all modules that base their behavior on some specific type of attribute declared on a message.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute(s) to collect from a message.</typeparam>
    public abstract class QueryModule<TMessageIn, TMessageOut, TAttribute> : QueryModule<TMessageIn, TMessageOut>
        where TMessageIn : class
        where TAttribute : MessageHandlerModuleAttribute
    {
        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>
        /// and <typeparamref name="TMessageOut"/> and invokes <see cref="Execute(TMessageIn, IEnumerable{TAttribute})" />.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public override TMessageOut Execute(TMessageIn message)
        {            
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var requestMessageAttributes = MessageHandlerModuleAttribute.SelectAttributesOfType<TAttribute>(message.GetType());
            var responseMessageAttributes = MessageHandlerModuleAttribute.SelectAttributesOfType<TAttribute>(typeof(TMessageOut));

            return Execute(message, requestMessageAttributes.Concat(responseMessageAttributes));
        }

        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="attributes">All attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>.</param>
        protected abstract TMessageOut Execute(TMessageIn message, IEnumerable<TAttribute> attributes);
    }
}
