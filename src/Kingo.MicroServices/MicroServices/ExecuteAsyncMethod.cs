using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents the <see cref="IQuery{TResponse}.ExecuteAsync"/> or
    /// <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/> method of a specific query.
    /// </summary>
    public abstract class ExecuteAsyncMethod : IAsyncMethod
    {
        private readonly Query _component;
        private readonly MethodAttributeProvider _attributeProvider;

        internal ExecuteAsyncMethod(Query component, QueryInterface @interface)
        {
            _component = component;
            _attributeProvider = @interface.CreateMethodAttributeProvider(component);
        }

        #region [====== Component ======]

        MicroProcessorComponent IAsyncMethod.Component =>
            Query;

        /// <summary>
        /// The message handler that implements this method.
        /// </summary>
        public Query Query =>
            _component;

        #endregion

        #region [====== IMethodAttributeProvider ======]

        /// <inheritdoc />
        public MethodInfo Info =>
            _attributeProvider.Info;

        /// <inheritdoc />
        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

        #endregion
    }
}
