using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class AttributeProvider<TMember> : IAttributeProvider<TMember>
        where TMember : MemberInfo
    {
        private static readonly ConcurrentDictionary<TMember, Attribute[]> _Attributes = new ConcurrentDictionary<TMember, Attribute[]>();        

        public AttributeProvider(TMember target)
        {
            Target = target;
        }

        public TMember Target
        {
            get;
        }

        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            try
            {
                attribute = GetAttributesOfType<TAttribute>().SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(Target, typeof(TAttribute));
            }
            return attribute != null;
        }

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            from attribute in _Attributes.GetOrAdd(Target, LoadAttributes)
            let desiredAttribute = attribute as TAttribute
            where desiredAttribute != null
            select desiredAttribute;

        private static Attribute[] LoadAttributes(TMember target) =>
            target.GetCustomAttributes().ToArray();

        private static Exception NewAmbiguousAttributeMatchException(TMember target, Type attributeType)
        {
            var messageFormat = ExceptionMessages.AttributeProvider_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, attributeType.Name, target.Name);
            return new InvalidOperationException(message);
        }
    }
}
