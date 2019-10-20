using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the <see cref="IMessageHandler{TMessage}.HandleAsync"/> method of a specific message handler.
    /// </summary>
    public class HandleAsyncMethod : IAsyncMethod
    {
        internal HandleAsyncMethod(HandleAsyncMethod method)
        {
            MessageHandler = method.MessageHandler;
            MethodInfo = method.MethodInfo;
            MessageParameterInfo = method.MessageParameterInfo;
            ContextParameterInfo = method.ContextParameterInfo;
        }

        internal HandleAsyncMethod(MessageHandlerComponent messageHandler, MessageHandlerInterface @interface) :
            this(messageHandler, @interface.ResolveMethodInfo(messageHandler)) { }

        private HandleAsyncMethod(MessageHandlerComponent messageHandler, MethodInfo info) :
            this(messageHandler, info, info.GetParameters()) { }

        private HandleAsyncMethod(MessageHandlerComponent messageHandler, MethodInfo info, ParameterInfo[] parameters)
        {
            MessageHandler = messageHandler;
            MethodInfo = info;
            MessageParameterInfo = parameters[0];
            ContextParameterInfo = parameters[1];
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{MessageHandler.Type.FriendlyName()}.{MethodInfo.Name}({MessageParameterInfo.ParameterType.FriendlyName()}, ...)";

        #region [====== IAsyncMethod ======]

        Type IAsyncMethod.ComponentType =>
            MessageHandler.Type;        

        /// <summary>
        /// The message handler that implements this method.
        /// </summary>
        public MessageHandlerComponent MessageHandler
        {
            get;
        }

        /// <inheritdoc />
        public MethodInfo MethodInfo
        {
            get;
        }
        
        /// <inheritdoc />
        public ParameterInfo MessageParameterInfo
        {
            get;
        }

        /// <inheritdoc />
        public ParameterInfo ContextParameterInfo
        {
            get;
        }

        #endregion

        #region [====== TryCreateEndpoint ======]

        private static readonly ConcurrentDictionary<Type, Func<HandleAsyncMethod, MicroProcessor, MicroServiceBusEndpointAttribute, MicroServiceBusEndpoint>> _EndpointConstructors =
            new ConcurrentDictionary<Type, Func<HandleAsyncMethod, MicroProcessor, MicroServiceBusEndpointAttribute, MicroServiceBusEndpoint>>();

        internal bool TryCreateMicroServiceBusEndpoint(MicroProcessor processor, out MicroServiceBusEndpoint endpoint)
        {
            if (MethodInfo.TryGetAttributeOfType(out MicroServiceBusEndpointAttribute attribute))
            {
                endpoint = CreateEndpoint(processor, attribute);
                return true;
            }
            endpoint = null;
            return false;
        }

        private MicroServiceBusEndpoint CreateEndpoint(MicroProcessor processor, MicroServiceBusEndpointAttribute attribute) =>
            _EndpointConstructors.GetOrAdd(MessageParameterInfo.ParameterType, CreateEndpointConstructor).Invoke(this, processor, attribute);

        private static Func<HandleAsyncMethod, MicroProcessor, MicroServiceBusEndpointAttribute, MicroServiceBusEndpoint> CreateEndpointConstructor(Type messageType)
        {
            var methodParameter = Expression.Parameter(typeof(HandleAsyncMethod), "method");
            var processorParameter = Expression.Parameter(typeof(MicroProcessor), "processor");
            var attributeParameter = Expression.Parameter(typeof(MicroServiceBusEndpointAttribute), "attribute");

            var endpointTypeDefinition = typeof(MicroServiceBusEndpoint<>);
            var endpointType = endpointTypeDefinition.MakeGenericType(messageType);
            var endpointConstructorParameters = new [] { typeof(HandleAsyncMethod), typeof(MicroProcessor), typeof(MicroServiceBusEndpointAttribute) };
            var endpointConstructor = endpointType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, endpointConstructorParameters, null);
                        
            var newEndpointExpression = Expression.New(endpointConstructor, methodParameter, processorParameter, attributeParameter);
            var createEndpointMethodExpression = Expression.Lambda<Func<HandleAsyncMethod, MicroProcessor, MicroServiceBusEndpointAttribute, MicroServiceBusEndpoint>>(
                newEndpointExpression,
                methodParameter,
                processorParameter,
                attributeParameter);

            return createEndpointMethodExpression.Compile();
        }

        #endregion
    }
}
