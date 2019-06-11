using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
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

        internal HandleAsyncMethod(MessageHandler component, MessageHandlerInterface @interface)
        {
            _component = component;
            _attributeProvider = @interface.CreateMethodAttributeProvider(component);
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
    }
}
