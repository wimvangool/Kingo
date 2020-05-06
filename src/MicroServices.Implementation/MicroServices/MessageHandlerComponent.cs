using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.MicroServices.Configuration;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represent a component that implements one or more variations of the <see cref="IMessageHandler{TMessage}"/> interface.
    /// </summary>
    public abstract class MessageHandlerComponent : MicroProcessorComponent, IHandleAsyncMethodFactory, IReadOnlyCollection<HandleAsyncMethod>
    {
        private readonly MessageHandlerInterface[] _interfaces;

        internal MessageHandlerComponent(MessageHandlerComponent component) :
            this(component, component._interfaces) { }

        internal MessageHandlerComponent(MicroProcessorComponent component, params MessageHandlerInterface[] interfaces) :
            base(component, interfaces.Select(@interface => @interface.Type))
        {
            _interfaces = interfaces;
        }        

        /// <summary>
        /// Returns the <see cref="IMessageHandler{TMessage}"/> interfaces that are implemented by this message handler.
        /// </summary>
        public IReadOnlyCollection<MessageHandlerInterface> Interfaces =>
            _interfaces;        

        /// <inheritdoc />
        public override string ToString() =>
            $"{Type.FriendlyName()} ({MicroProcessorComponentInterface.ToString(_interfaces)}";

        #region [====== IHandleAsyncMethodFactory.CreateMicroServiceBusEndpoints(MicroProcessor) ======]

        IEnumerable<MicroServiceBusEndpoint> IHandleAsyncMethodFactory.CreateMicroServiceBusEndpoints(MicroProcessor processor)
        {
            foreach (var method in Methods())
            {
                if (method.TryCreateMicroServiceBusEndpoint(processor, out var endpoint))
                {
                    yield return endpoint;
                }
            }
        }

        int IReadOnlyCollection<HandleAsyncMethod>.Count =>
            _interfaces.Length;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <inheritdoc />
        public IEnumerator<HandleAsyncMethod> GetEnumerator() =>
            Methods().GetEnumerator();

        private IEnumerable<HandleAsyncMethod> Methods() =>
            _interfaces.Select(@interface => @interface.CreateMethod(this));

        #endregion

        #region [====== IHandleAsyncMethodFactory.CreateMethodsFor<TMessage> ======]

        private static readonly ConcurrentDictionary<MessageHandlerInterface, Func<object, object>> _MessageHandlerFactories =
            new ConcurrentDictionary<MessageHandlerInterface, Func<object, object>>();        

        IEnumerable<HandleAsyncMethod<TEvent>> IHandleAsyncMethodFactory.CreateInternalEventBusEndpointsFor<TEvent>(IServiceProvider serviceProvider)
        {
            // This LINQ construct first selects all message handler interface definitions that are compatible with
            // the specified message. Then it will dynamically create the correct message handler type for each match
            // and return it.
            //
            // Example:
            //   When the class implements both IMessageHandler<object> and IMessageHandler<SomeMessage>, then if
            //   message is of type SomeMessage, two message-handler instances are returned, one for each
            //   implementation.
            foreach (var @interface in Interfaces.Where(@interface => @interface.MessageType.IsAssignableFrom(typeof(TEvent))))
            {
                if (IsInternalServiceBusEndpoint(@interface))
                {
                    yield return CreateHandleAsyncMethod<TEvent>(@interface, serviceProvider);
                }
            }
        }

        internal virtual bool IsInternalServiceBusEndpoint(MessageHandlerInterface @interface)
        {
            if (@interface.CreateMethod(this).MethodInfo.TryGetAttributeOfType<MicroServiceBusEndpointAttribute>(out var attribute))
            {
                return attribute.Types.HasFlag(MicroServiceBusEndpointTypes.Internal);
            }
            return false;
        }

        private HandleAsyncMethod<TMessage> CreateHandleAsyncMethod<TMessage>(MessageHandlerInterface @interface, IServiceProvider serviceProvider) =>
            new HandleAsyncMethod<TMessage>(CreateMessageHandler<TMessage>(@interface, serviceProvider), this, @interface);

        private IMessageHandler<TMessage> CreateMessageHandler<TMessage>(MessageHandlerInterface @interface, IServiceProvider serviceProvider) =>
            CreateMessageHandler(@interface, serviceProvider) as IMessageHandler<TMessage>;

        private object CreateMessageHandler(MessageHandlerInterface @interface, IServiceProvider serviceProvider) =>
            _MessageHandlerFactories.GetOrAdd(@interface, CreateMessageHandlerFactory).Invoke(ResolveMessageHandler(serviceProvider));

        private static Func<object, object> CreateMessageHandlerFactory(MessageHandlerInterface @interface)
        {
            // In order to support message handlers with contravariant IMessageHandler<TMessage>-implementations, the resolved instance is wrapped
            // inside an MessageHandlerDecorator<TActual> where TActual is the type of the generic type parameter of the specified interface. This ensures that
            // the correct interface method of the handler is called at runtime.
            //
            // For example, suppose a message handler is declared as follows...
            //
            // public sealed class ObjectHandler : IMessageHandler<object>
            // {
            //     public Task HandleAsync(object message, MessageHandlerContext context) 
            //     {
            //         ...
            //     }    
            // }
            //
            // ...and this method is called with a TMessage of type 'string', then the code will do something along the following lines:
            //            
            // - return new MessageHandlerDecorator<object>(Resolve<ObjectHandler>());            
            var messageHandlerParameter = Expression.Parameter(typeof(object), "messageHandler");
            var messageHandlerOfExpectedType = Expression.Convert(messageHandlerParameter, @interface.Type);

            var decoratorTypeDefinition = typeof(MessageHandlerDecorator<>);
            var decoratorType = decoratorTypeDefinition.MakeGenericType(@interface.MessageType);
            var decoratorConstructorParameters = new[] { @interface.Type };
            var decoratorConstructor = decoratorType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, decoratorConstructorParameters, null);
            var newDecoratorExpression = Expression.New(decoratorConstructor, messageHandlerOfExpectedType);
            var newMessageHandlerExpression = Expression.Lambda<Func<object, object>>(newDecoratorExpression, messageHandlerParameter);

            return newMessageHandlerExpression.Compile();
        }

        internal abstract object ResolveMessageHandler(IServiceProvider serviceProvider);

        #endregion
    }
}
