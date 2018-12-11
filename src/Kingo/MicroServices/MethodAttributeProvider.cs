using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class MethodAttributeProvider : IAttributeProvider<MethodInfo>
    {
        private readonly AttributeProvider<MethodInfo> _attributeProvider;

        public MethodAttributeProvider(MethodInfo target)
        {
            _attributeProvider = new AttributeProvider<MethodInfo>(target);
        }

        public MethodInfo Target =>
            _attributeProvider.Target;

        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();                

        #region [====== Factory Methods ======]

        private const string _HandleAsync = nameof(IMessageHandler<object>.HandleAsync);
        private const string _ExecuteAsync = nameof(IQuery<object>.ExecuteAsync);

        public static MethodAttributeProvider FromMessageHandler<TMessage>(IMessageHandler<TMessage> handler) =>
            FromMessageHandler(handler.GetType(), typeof(IMessageHandler<TMessage>), typeof(TMessage));

        internal static MethodAttributeProvider FromMessageHandler(Type handlerType, Type interfaceType) =>
            FromMessageHandler(handlerType, interfaceType, interfaceType.GetGenericArguments()[0]);

        private static MethodAttributeProvider FromMessageHandler(Type handlerType, Type interfaceType, Type messageType) =>
            FromInterfaceMethod(handlerType, interfaceType, _HandleAsync, messageType, typeof(MessageHandlerContext));

        public static MethodAttributeProvider FromQuery<TMessageOut>(IQuery<TMessageOut> query) =>
            FromInterfaceMethod(query.GetType(), typeof(IQuery<TMessageOut>), _ExecuteAsync, typeof(QueryContext));

        public static MethodAttributeProvider FromQuery<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query) =>
            FromInterfaceMethod(query.GetType(), typeof(IQuery<TMessageIn, TMessageOut>), _ExecuteAsync, typeof(TMessageIn), typeof(QueryContext));

        private static MethodAttributeProvider FromInterfaceMethod(Type type, Type interfaceType, string methodName, params Type[] methodParameters)
        {            
            var methods =
                from method in type.GetInterfaceMap(interfaceType).TargetMethods
                where Normalize(method.Name) == methodName && IsMatch(method.GetParameters(), methodParameters)
                select method;

            try
            {
                return new MethodAttributeProvider(methods.Single());
            }     
            catch (InvalidOperationException)
            {
                throw NewInterfaceMethodNotFoundException(type, interfaceType, methodName, methodParameters);
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

        private static Exception NewInterfaceMethodNotFoundException(Type type, Type interfaceType, string methodName, IEnumerable<Type> methodParameters)
        {
            var messageFormat = ExceptionMessages.MethodAttributeProvider_InterfaceMethodNotFound;
            var message = string.Format(messageFormat, interfaceType.FriendlyName(), methodName, methodParameters.FriendlyNames(), type.FriendlyName());
            return new InvalidOperationException(message);
        }        

        private static bool IsMatch(IReadOnlyList<ParameterInfo> parameters, IReadOnlyList<Type> expectedTypes)
        {
            if (parameters.Count != expectedTypes.Count)
            {                
                return false;
            }
            for (int index = 0; index < parameters.Count; index++)
            {                
                var parameter = parameters[index];
                if (parameter.ParameterType != expectedTypes[index])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
