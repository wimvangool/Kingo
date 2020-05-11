using System;
using System.Collections.Generic;

namespace Kingo.MicroServices
{
    internal abstract class MessageAttributeConsumer
    {
        private readonly Dictionary<Type, MessageAttribute> _attributes;

        protected MessageAttributeConsumer(IDictionary<Type, MessageAttribute> attributes)
        {
            _attributes = new Dictionary<Type, MessageAttribute>(attributes);
        }

        protected bool TryGetMessageAttribute(Type contentType, out MessageAttribute attribute)
        {
            if (TryGetMessageAttributeForType(contentType, out attribute))
            {
                return true;
            }
            if (contentType.BaseType == null)
            {
                return false;
            }
            return TryGetMessageAttribute(contentType.BaseType, out attribute);
        }

        private bool TryGetMessageAttributeForType(Type contentType, out MessageAttribute attribute) =>
            _attributes.TryGetValue(contentType, out attribute) || MessageAttribute.TryGetMessageAttribute(contentType, false, out attribute);
    }
}
