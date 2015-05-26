using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;

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

        #region [====== Class Attributes ======]

        internal bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            try
            {
                attribute = GetClassAttributesOfType<TAttribute>().SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(ClassType, typeof(TAttribute));
            }
            return attribute != null;
        }

        internal IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return from attribute in _ClassAttributes.GetOrAdd(ClassType, LoadClassAttributes)
                   let desiredAttribute = attribute as TAttribute
                   where desiredAttribute != null
                   select desiredAttribute;
        }

        private static readonly ConcurrentDictionary<Type, Attribute[]> _ClassAttributes =
            new ConcurrentDictionary<Type, Attribute[]>();

        private static Attribute[] LoadClassAttributes(Type classType)
        {
            return classType.GetCustomAttributes(true).Cast<Attribute>().ToArray();
        }

        #endregion

        #region [====== Method Attributes ======]

        internal bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            try
            {
                attribute = GetMethodAttributesOfType<TAttribute>().SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(ClassType, typeof(TAttribute));
            }
            return attribute != null;
        }

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

        #endregion

        private static Exception NewAmbiguousAttributeMatchException(Type messageHandlerType, Type attributeType)
        {
            var messageFormat = ExceptionMessages.MessageHandler_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, messageHandlerType, attributeType);
            return new AmbiguousMatchException(message);
        }
    }
}
