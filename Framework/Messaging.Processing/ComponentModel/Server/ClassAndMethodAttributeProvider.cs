using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Syztem.Resources;

namespace Syztem.ComponentModel.Server
{
    internal sealed class ClassAndMethodAttributeProvider : IEquatable<ClassAndMethodAttributeProvider>, IMessageHandlerOrQuery
    {
        private readonly Type _classType;
        private readonly Type _interfaceType;

        internal ClassAndMethodAttributeProvider(Type classType, Type interfaceType)
        {
            _classType = classType;
            _interfaceType = interfaceType;
        }

        private InterfaceMapping GetInterfaceMapping()
        {
            return _classType.GetInterfaceMap(_interfaceType);
        }

        #region [====== Equals & GetHashCode ======]

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as ClassAndMethodAttributeProvider);
        }

        /// <inheritdoc />
        public bool Equals(ClassAndMethodAttributeProvider other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (ReferenceEquals(other, this))
            {
                return true;
            }
            return _classType == other._classType && _interfaceType == other._interfaceType;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Of(_classType, _interfaceType);
        }

        #endregion

        #region [====== Class Attributes ======]

        public bool TryGetClassAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            try
            {
                attribute = GetClassAttributesOfType<TAttribute>().SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(_classType, typeof(TAttribute));
            }
            return attribute != null;
        }

        public IEnumerable<TAttribute> GetClassAttributesOfType<TAttribute>() where TAttribute : class
        {
            return from attribute in _ClassAttributes.GetOrAdd(_classType, LoadClassAttributes)
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

        public bool TryGetMethodAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class
        {
            try
            {
                attribute = GetMethodAttributesOfType<TAttribute>().SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw NewAmbiguousAttributeMatchException(_classType, typeof(TAttribute));
            }
            return attribute != null;
        }

        public IEnumerable<TAttribute> GetMethodAttributesOfType<TAttribute>() where TAttribute : class
        {
            return from attribute in _MethodAttributes.GetOrAdd(this, LoadMethodAttributes)
                   let desiredAttribute = attribute as TAttribute
                   where desiredAttribute != null
                   select desiredAttribute;
        }

        private static readonly ConcurrentDictionary<ClassAndMethodAttributeProvider, Attribute[]> _MethodAttributes =
            new ConcurrentDictionary<ClassAndMethodAttributeProvider, Attribute[]>();

        private static Attribute[] LoadMethodAttributes(ClassAndMethodAttributeProvider provider)
        {
            // TargetMethods[0] points to the Handle(TMessage) or Execute(TMessage) method.           
            return provider.GetInterfaceMapping().TargetMethods[0]
                .GetCustomAttributes(true)
                .Cast<Attribute>()
                .ToArray();
        }

        #endregion

        internal static bool TryGetStrategyFromAttribute<TAttribute>(IMessageHandlerOrQuery handler, out TAttribute attribute) where TAttribute : class
        {
            return handler.TryGetMethodAttributeOfType(out attribute) || handler.TryGetClassAttributeOfType(out attribute);
        } 

        private static Exception NewAmbiguousAttributeMatchException(Type messageHandlerType, Type attributeType)
        {
            var messageFormat = ExceptionMessages.MessageHandler_AmbiguousAttributeMatch;
            var message = string.Format(messageFormat, messageHandlerType, attributeType);
            return new AmbiguousMatchException(message);
        }
    }
}
