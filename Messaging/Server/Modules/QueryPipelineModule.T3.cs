using System.Collections.Generic;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Serves as a base class for all modules that base their behavior on some specific type of attribute declared on a message.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the message that is consumed by this query.</typeparam>
    /// <typeparam name="TMessageOut">Type of the message that is returned by this query.</typeparam>
    /// <typeparam name="TAttribute">Type of the attribute(s) to collect from a message.</typeparam>
    public abstract class QueryPipelineModule<TMessageIn, TMessageOut, TAttribute> : QueryPipelineModule<TMessageIn, TMessageOut>
        where TMessageIn : class 
        where TAttribute : Attribute
    {
        /// <summary>
        /// Retrieves all attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>
        /// and invokes <see cref="Execute(TMessageIn, IEnumerable{TAttribute})" />.
        /// </summary>
        /// <param name="message">>Message containing the parameters of this query.</param>
        protected override TMessageOut Execute(TMessageIn message)
        {
            return Execute(message, MessageProcessor.SelectAttributesOfType<TAttribute>(message.GetType()));
        }

        /// <summary>
        /// Handles the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Message containing the parameters of this query.</param>
        /// <param name="attributes">All attributes of type <typeparamref name="TAttribute"/> declared on <paramref name="message"/>.</param>
        protected abstract TMessageOut Execute(TMessageIn message, IEnumerable<TAttribute> attributes);
    }
}
