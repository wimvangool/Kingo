using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class TypeAttributeProvider : ITypeAttributeProvider
    {
        #region [====== NullProvider ======]

        private sealed class NullProvider : ITypeAttributeProvider
        {
            public bool TryGetTypeAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
            {
                attribute = null;
                return false;
            }

            public IEnumerable<TAttribute> GetTypeAttributesOfType<TAttribute>() where TAttribute : class =>
                Enumerable.Empty<TAttribute>();
        }

        #endregion

        public static readonly ITypeAttributeProvider None = new NullProvider();
        private static readonly ConcurrentDictionary<Type, Attribute[]> _Attributes = new ConcurrentDictionary<Type, Attribute[]>();

        public TypeAttributeProvider(Type type)
        {
            Type = type;
        }

        public Type Type
        {
            get;
        }

        public bool TryGetTypeAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            try
            {
                attribute = GetTypeAttributesOfType<TAttribute>().SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(Type, typeof(TAttribute));
            }
            return attribute != null;
        }

        public IEnumerable<TAttribute> GetTypeAttributesOfType<TAttribute>() where TAttribute : class =>
            from attribute in _Attributes.GetOrAdd(Type, LoadAttributes)
            let desiredAttribute = attribute as TAttribute
            where desiredAttribute != null
            select desiredAttribute;

        private static Attribute[] LoadAttributes(Type type) =>
            type.GetCustomAttributes(true).Cast<Attribute>().ToArray();

        private static Exception NewAmbiguousAttributeMatchException(Type classType, Type attributeType)
        {            
            var messageFormat = ExceptionMessages.TypeAttributeProvider_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, attributeType.Name, classType.Name);
            return new InvalidOperationException(message);
        }
    }
}
