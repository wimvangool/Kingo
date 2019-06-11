using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.MicroServices;

namespace Kingo.Reflection
{
    internal sealed class MethodAttributeProvider : IMethodAttributeProvider
    {
        private readonly AttributeProvider<MethodInfo> _attributeProvider;

        public MethodAttributeProvider(MethodInfo info)
        {
            _attributeProvider = new AttributeProvider<MethodInfo>(info);
        }

        public MethodInfo Info =>
            _attributeProvider.Target;

        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();                        
    }
}
