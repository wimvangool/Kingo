using System;
using System.Collections.Generic;
using Kingo.MicroServices;

namespace Kingo.Reflection
{
    internal sealed class TypeAttributeProvider : IAttributeProvider<Type>
    {
        private readonly AttributeProvider<Type> _attributeProvider;

        public TypeAttributeProvider(Type target)
        {           
            _attributeProvider = new AttributeProvider<Type>(target);
        }

        public Type Target =>
            _attributeProvider.Target;

        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();
    }
}
