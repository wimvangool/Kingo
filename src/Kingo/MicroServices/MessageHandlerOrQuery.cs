using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base class for any message handler or query that is able to process a message and provides access
    /// to its own attributes.
    /// </summary>
    public abstract class MessageHandlerOrQuery : IAttributeProvider<Type>
    {
        #region [====== IAttributeProvider<Type> ======]

        /// <inheritdoc />
        Type IAttributeProvider<Type>.Target =>
            Type;

        /// <summary>
        /// Returns the message handler or query type.
        /// </summary>
        public abstract Type Type
        {
            get;
        }

        /// <inheritdoc />
        public abstract bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute)
            where TAttribute : class;

        /// <inheritdoc />
        public abstract IEnumerable<TAttribute> GetAttributesOfType<TAttribute>()
            where TAttribute : class;

        #endregion        
    }
}
