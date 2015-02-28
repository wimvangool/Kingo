using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all modules that base their behavior on some specific type of attribute declared on a message.
    /// </summary>    
    /// <typeparam name="TAttribute">Type of the attribute(s) to collect from a message.</typeparam>
    public abstract class QueryModule<TAttribute> : QueryModule where TAttribute : Attribute
    {
        /// <inheritdoc />
        protected override TMessageOut InvokeQuery<TMessageOut>(IQuery<TMessageOut> query)
        {
            return InvokeQuery(query, query.MessageIn.SelectAttributesOfType<TAttribute>());
        }

        /// <summary>
        /// Invokes the specified <paramref name="query"/> while adding specific pipeline logic.
        /// </summary>
        /// <typeparam name="TMessageOut">Type of the result of <paramref name="query"/>.</typeparam>
        /// <param name="query">The query to invoke.</param>
        /// <param name="attributes">All attributes of type <typeparamref name="TAttribute"/> declared on the incoming message.</param>
        protected abstract TMessageOut InvokeQuery<TMessageOut>(IQuery<TMessageOut> query, IEnumerable<TAttribute> attributes)
            where TMessageOut : class, IMessage<TMessageOut>;
    }
}
