using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Kingo.Reflection;

namespace Kingo.MicroServices
{        
    internal sealed class MessageHandlerType : MessageHandler, IHandleAsyncMethodFactory
    {        
        private MessageHandlerType(MicroProcessorComponent component, MessageHandlerInterface[] interfaces) :
            base(component, interfaces) { }

        public IEnumerable<HandleAsyncMethod<TMessage>> CreateMethodsFor<TMessage>(MicroProcessorOperationKinds operationKind, IServiceProvider serviceProvider)
        {            
            if (operationKind.IsSupportedBy(GetSupportedOperations()))
            {
                // This LINQ construct first selects all message handler interface definitions that are compatible with
                // the specified message. Then it will dynamically create the correct message handler type for each match
                // and return it.
                //
                // Example:
                //   When the class implements both IMessageHandler<object> and IMessageHandler<SomeMessage>, then if
                //   message is of type SomeMessage, two message-handler instances are returned, one for each
                //   implementation.
                return
                    from messageHandlerInterface in Interfaces
                    where messageHandlerInterface.MessageType.IsInstanceOfType(typeof(TMessage))
                    select CreateHandleAsyncMethod<TMessage>(messageHandlerInterface, serviceProvider);
            }
            return Enumerable.Empty<HandleAsyncMethod<TMessage>>();
        }

        private HandleAsyncMethod<TMessage> CreateHandleAsyncMethod<TMessage>(MessageHandlerInterface @interface, IServiceProvider serviceProvider) =>
            new HandleAsyncMethod<TMessage>(CreateMessageHandler<TMessage>(@interface, serviceProvider), this, @interface);

        private static readonly ConcurrentDictionary<MessageHandlerInterface, Func<object, object>> _MessageHandlerFactories =
            new ConcurrentDictionary<MessageHandlerInterface, Func<object, object>>();

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
            var decoratorConstructorParameters = new [] { @interface.Type };
            var decoratorConstructor = decoratorType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, decoratorConstructorParameters, null);
            var newDecoratorExpression = Expression.New(decoratorConstructor, messageHandlerOfExpectedType);
            var newMessageHandlerExpression = Expression.Lambda<Func<object, object>>(newDecoratorExpression, messageHandlerParameter);                

            return newMessageHandlerExpression.Compile();
        }

        private object ResolveMessageHandler(IServiceProvider serviceProvider)
        {
            var messageHandler = serviceProvider?.GetService(Type);
            if (messageHandler == null)
            {
                throw NewCouldNotResolveMessageHandlerException(Type);
            }
            return messageHandler;
        }

        private static Exception NewCouldNotResolveMessageHandlerException(Type messageHandlerType)
        {
            var messageFormat = ExceptionMessages.MessageHandlerClass_CouldNotResolveMessageHandler;
            var message = string.Format(messageFormat, messageHandlerType.FriendlyName());
            return new InvalidOperationException(message);
        }

        #region [====== FromInstance ======]

        public static MessageHandlerType FromInstance(object messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }
            var component = new MicroProcessorComponent(messageHandler.GetType());
            var interfaces = MessageHandlerInterface.FromComponent(component).ToArray();
            return new MessageHandlerType(component, interfaces);
        }

        #endregion

        #region [====== FromComponents ======]               

        public static IEnumerable<MessageHandlerType> FromComponents(IEnumerable<MicroProcessorComponent> components)
        {                  
            foreach (var component in components)
            {
                if (IsMessageHandlerComponent(component, out var messageHandler))
                {
                    yield return messageHandler;
                }
            }            
        }

        internal static bool IsMessageHandlerComponent(Type type, out MessageHandlerType messageHandler)
        {
            if (CanBeCreatedFrom(type))
            {
                return IsMessageHandlerComponent(new MicroProcessorComponent(type), out messageHandler);
            }
            messageHandler = null;
            return false;
        }

        private static bool IsMessageHandlerComponent(MicroProcessorComponent component, out MessageHandlerType messageHandler)
        {
            var interfaces = MessageHandlerInterface.FromComponent(component).ToArray();
            if (interfaces.Length == 0)
            {
                messageHandler = null;
                return false;
            }
            messageHandler = new MessageHandlerType(component, interfaces);
            return true;                                   
        }                      

        #endregion        
    }
}
