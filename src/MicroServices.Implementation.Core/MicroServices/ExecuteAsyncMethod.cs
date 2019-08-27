using System.Collections.Generic;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents the <see cref="IQuery{TResponse}.ExecuteAsync"/> or
    /// <see cref="IQuery{TRequest, TResponse}.ExecuteAsync"/> method of a specific query.
    /// </summary>
    public class ExecuteAsyncMethod : IAsyncMethod
    {
        private readonly Query _component;
        private readonly MethodAttributeProvider _attributeProvider;
        private readonly IParameterAttributeProvider _messageParameter;
        private readonly IParameterAttributeProvider _contextParameter;

        internal ExecuteAsyncMethod(Query component, QueryInterface @interface) :
            this(component, @interface.CreateMethodAttributeProvider(component)) { }

        private ExecuteAsyncMethod(Query component, MethodAttributeProvider attributeProvider) :
            this(component, attributeProvider, attributeProvider.Info.GetParameters()) { }

        private ExecuteAsyncMethod(Query component, MethodAttributeProvider attributeProvider, ParameterInfo[] parameters)
        {
            _component = component;
            _attributeProvider = attributeProvider;

            if (parameters.Length == 1)
            {
                _messageParameter = null;
                _contextParameter = new ParameterAttributeProvider(parameters[0]);
            }
            else
            {
                _messageParameter = new ParameterAttributeProvider(parameters[0]);
                _contextParameter = new ParameterAttributeProvider(parameters[1]);
            }
        }

        #region [====== Component ======]

        ITypeAttributeProvider IAsyncMethod.Component =>
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

        #region [====== Parameters ======]

        /// <inheritdoc />
        public IParameterAttributeProvider MessageParameter =>
            _messageParameter;

        /// <inheritdoc />
        public IParameterAttributeProvider ContextParameter =>
            _contextParameter;       

        #endregion
    }
}
