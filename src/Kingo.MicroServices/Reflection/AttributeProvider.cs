using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Reflection
{
    internal abstract class AttributeProvider : IAttributeProvider
    {
        private static readonly ConcurrentDictionary<object, Attribute[]> _Attributes = new ConcurrentDictionary<object, Attribute[]>();

        protected abstract object Target
        {
            get;
        }

        protected abstract string TargetName
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
                throw NewAmbiguousAttributeMatchException(TargetName, typeof(TAttribute));
            }
            return attribute != null;
        }

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            from attribute in _Attributes.GetOrAdd(Target, target => LoadAttributes())
            let desiredAttribute = attribute as TAttribute
            where desiredAttribute != null
            select desiredAttribute;

        protected abstract Attribute[] LoadAttributes();            

        private static Exception NewAmbiguousAttributeMatchException(string targetName, Type attributeType)
        {
            var messageFormat = ExceptionMessages.AttributeProvider_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, attributeType.Name, targetName);
            return new InvalidOperationException(message);
        }
    }
}
