using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly MessageHandler _component;
        private readonly MethodAttributeProvider _attributeProvider;
        private readonly IParameterAttributeProvider _messageParameter;
        private readonly IParameterAttributeProvider _contextParameter;

        internal HandleAsyncMethod(HandleAsyncMethod method)
        {
            _component = method._component;
            _attributeProvider = method._attributeProvider;
            _messageParameter = method._messageParameter;
            _contextParameter = method._contextParameter;
        }

        internal HandleAsyncMethod(MessageHandler component, MessageHandlerInterface @interface) :
            this(component, @interface.CreateMethodAttributeProvider(component)) { }

        private HandleAsyncMethod(MessageHandler component, MethodAttributeProvider attributeProvider) :
            this(component, attributeProvider, attributeProvider.Info.GetParameters()) { }

        private HandleAsyncMethod(MessageHandler component, MethodAttributeProvider attributeProvider, ParameterInfo[] parameters)
        {
            _component = component;
            _attributeProvider = attributeProvider;
            _messageParameter = new ParameterAttributeProvider(parameters[0]);
            _contextParameter = new ParameterAttributeProvider(parameters[1]);
        }

        public override string ToString() =>
            $"{MessageHandler.Type.FriendlyName()}.{Info.Name}({MessageParameter.Type.FriendlyName()}, ...)";

        #region [====== Component ======]

        MicroProcessorComponent IAsyncMethod.Component =>
            MessageHandler;        

        /// <summary>
        /// The message handler that implements this method.
        /// </summary>
        public MessageHandler MessageHandler =>
            _component;

        #endregion

        #region [====== IMethodAttributeProvider ======]

        /// <inheritdoc />
        public MethodInfo Info =>
            _attributeProvider.Info;

        /// <inheritdoc />
        public bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) where TAttribute : class =>
            _attributeProvider.TryGetAttributeOfType(out attribute);

        /// <inheritdoc />
        public IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() where TAttribute : class =>
            _attributeProvider.GetAttributesOfType<TAttribute>();

        #endregion

        #region [====== Parameters ======]

        /// <inheritdoc />
        public IParameterAttributeProvider MessageParameter =>
            _messageParameter;

        /// <inheritdoc />
        public IParameterAttributeProvider ContextParameter =>
            _contextParameter;

        #endregion

        #region [====== TryCreateEndpoint ======]

        private static readonly ConcurrentDictionary<Type, Func<HandleAsyncMethod, MicroProcessor, EndpointAttribute, HandleAsyncMethodEndpoint>> _EndpointConstructors =
            new ConcurrentDictionary<Type, Func<HandleAsyncMethod, MicroProcessor, EndpointAttribute, HandleAsyncMethodEndpoint>>();

        internal bool TryCreateEndpoint(MicroProcessor processor, out HandleAsyncMethodEndpoint endpoint)
        {
            if (TryGetAttributeOfType(out EndpointAttribute attribute))
            {
                endpoint = CreateEndpoint(processor, attribute);
                return true;
            }
            endpoint = null;
            return false;
        }

        private HandleAsyncMethodEndpoint CreateEndpoint(MicroProcessor processor, EndpointAttribute attribute) =>
            _EndpointConstructors.GetOrAdd(MessageParameter.Type, CreateEndpointConstructor).Invoke(this, processor, attribute);

        private static Func<HandleAsyncMethod, MicroProcessor, EndpointAttribute, HandleAsyncMethodEndpoint> CreateEndpointConstructor(Type messageType)
        {
            var methodParameter = Expression.Parameter(typeof(HandleAsyncMethod), "method");
            var processorParameter = Expression.Parameter(typeof(MicroProcessor), "processor");
            var attributeParameter = Expression.Parameter(typeof(EndpointAttribute), "attribute");

            var endpointTypeDefinition = typeof(HandleAsyncMethodEndpoint<>);
            var endpointType = endpointTypeDefinition.MakeGenericType(messageType);
            var endpointConstructorParameters = new [] { typeof(HandleAsyncMethod), typeof(MicroProcessor), typeof(EndpointAttribute) };
            var endpointConstructor = endpointType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, endpointConstructorParameters, null);
                        
            var newEndpointExpression = Expression.New(endpointConstructor, methodParameter, processorParameter, attributeParameter);
            var createEndpointMethodExpression = Expression.Lambda<Func<HandleAsyncMethod, MicroProcessor, EndpointAttribute, HandleAsyncMethodEndpoint>>(
                newEndpointExpression,
                methodParameter,
                processorParameter,
                attributeParameter);

            return createEndpointMethodExpression.Compile();
        }

        #endregion
    }
}
