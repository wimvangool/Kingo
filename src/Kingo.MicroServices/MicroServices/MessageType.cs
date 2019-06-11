using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal class MessageType : IMessageType
    {
        private readonly TypeAttributeProvider _attributeProvider;
        private readonly MessageKind _kind;

        public MessageType(Type type, MessageKind kind)
        {
            _attributeProvider = new TypeAttributeProvider(type);
            _kind = kind;
        }

        #region [====== ITypeAttributeProvider ======]

        public Type Type =>
            _attributeProvider.Type;

        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

        #endregion

        #region [====== IMessage ======]

        public MessageKind Kind =>
            _kind;

        #endregion
    }
}
