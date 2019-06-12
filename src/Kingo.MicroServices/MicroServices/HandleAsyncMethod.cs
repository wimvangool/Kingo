using System.Collections.Generic;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents the <see cref="IMessageHandler{TMessage}.HandleAsync"/> method of a
    /// specific message handler.
    /// </summary>
    public abstract class HandleAsyncMethod : IAsyncMethod
    {
        private readonly MessageHandler _component;
        private readonly MethodAttributeProvider _attributeProvider;
        private readonly IParameterAttributeProvider _messageParameter;
        private readonly IParameterAttributeProvider _contextParameter;

        internal HandleAsyncMethod(MessageHandler component, MessageHandlerInterface @interface) :
            this(component, @interface.CreateMethodAttributeProvider(component)) { }

        private HandleAsyncMethod(MessageHandler component, MethodAttributeProvider attributeProvider) :
            this(component, attributeProvider, attributeProvider.Info.GetParameters()) { }

        private HandleAsyncMethod(MessageHandler component, MethodAttributeProvider attributeProvider, ParameterInfo[] parameters)
        {
            _component = component;
            _attributeProvider = attributeProvider;
            _messageParameter = new ParameterAttributeProvider(parameters[0]);
            _contextParameter = new ParameterAttributeProvider(parameters[1]);
        }

        #region [====== Component ======]

        MicroProcessorComponent IAsyncMethod.Component =>
            MessageHandler;        

        /// <summary>
        /// The message handler that implements this method.
        /// </summary>
        public MessageHandler MessageHandler =>
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
