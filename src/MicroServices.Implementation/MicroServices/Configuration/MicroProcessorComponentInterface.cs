using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented, represents a specific variant of the <see cref="IMessageHandler{TMessage}"/>, <see cref="IQuery{TResponse}"/>
    /// or <see cref="IQuery{TRequest, TResponse}"/> interface.
    /// </summary>
    public abstract class MicroProcessorComponentInterface : IEquatable<MicroProcessorComponentInterface>
    {        
        internal MicroProcessorComponentInterface(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Type of the interface.
        /// </summary>
        public Type Type
        {
            get;
        }

        internal abstract string MethodName
        {
            get;
        }        

        #region [====== Equals, GetHashCode & ToString ======]

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            Equals(obj as MicroProcessorComponentInterface);

        /// <inheritdoc />
        public bool Equals(MicroProcessorComponentInterface other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Type == other.Type;
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            Type.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            Type.FriendlyName();

        internal static string ToString(IEnumerable<MicroProcessorComponentInterface> interfaces) =>
            string.Join(", ", interfaces.Select(@interface => @interface.ToString()));

        #endregion

        #region [====== CreateMethodAttributeProvider ======]

        internal MethodInfo ResolveMethodInfo(MicroProcessorComponent component) =>
            ResolveMethodInfo(component.Type, Type, MethodName);

        private static MethodInfo ResolveMethodInfo(Type type, Type interfaceType, string methodName)
        {
            var methods =
                from method in type.GetInterfaceMap(interfaceType).TargetMethods
                where Normalize(method.Name) == methodName
                select method;

            try
            {
                return methods.Single();
            }
            catch (InvalidOperationException)
            {
                throw NewInterfaceMethodNotFoundException(type, interfaceType, methodName);
            }
        }

        private static string Normalize(string methodName)
        {
            var dotIndex = methodName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
            if (dotIndex < 0)
            {
                return methodName;
            }
            return methodName.Substring(dotIndex + 1);
        }

        private static Exception NewInterfaceMethodNotFoundException(Type type, Type interfaceType, string methodName)
        {
            var messageFormat = ExceptionMessages.MicroProcessorComponentInterface_InterfaceMethodNotFound;
            var message = string.Format(messageFormat, interfaceType.FriendlyName(), methodName, type.FriendlyName());
            return new InvalidOperationException(message);
        }      

        #endregion
    }
}
