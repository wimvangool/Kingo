using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Server
{
    internal abstract class MessageHandlerInstance : IEquatable<MessageHandlerInstance>
    {
        protected abstract Type ClassType
        {
            get;
        }

        protected abstract Type InterfaceType
        {
            get;
        }

        private InterfaceMapping GetInterfaceMapping()
        {
            return ClassType.GetInterfaceMap(InterfaceType);
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as MessageHandlerInstance);
        }

        /// <inheritdoc />
        public bool Equals(MessageHandlerInstance other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return ClassType == other.ClassType && InterfaceType == other.InterfaceType;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(ClassType, InterfaceType);
        }

        #endregion

        internal IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return from attribute in _MethodAttributes.GetOrAdd(this, LoadMethodAttributes)
                   let desiredAttribute = attribute as TAttribute
                   where desiredAttribute != null
                   select desiredAttribute;
        }

        private static readonly ConcurrentDictionary<MessageHandlerInstance, Attribute[]> _MethodAttributes =
            new ConcurrentDictionary<MessageHandlerInstance, Attribute[]>();

        private static Attribute[] LoadMethodAttributes(MessageHandlerInstance instance)
        {
            // TargetMethods[0] points to the Handle(TMessage) method.           
            return instance.GetInterfaceMapping().TargetMethods[0]
                .GetCustomAttributes(true)
                .Cast<Attribute>()
                .ToArray();
        }        
    }
}
