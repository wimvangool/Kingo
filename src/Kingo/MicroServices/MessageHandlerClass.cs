using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kingo.MicroServices
{    
    internal sealed class MessageHandlerClass
    {        
        private MessageHandlerClass(Type type, Type[] interfaces, IMessageHandlerConfiguration configuration)
        {           
            Type = type;
            Interfaces = interfaces;
            Configuration = configuration;            
        }

        public Type Type
        {
            get;
        }

        public IReadOnlyList<Type> Interfaces
        {
            get;
        }

        public IMessageHandlerConfiguration Configuration
        {
            get;
        }
        
        public override string ToString() =>
            $"{Type.FriendlyName()} ({Interfaces.Count} interface(s) implemented) - Configuration = {Configuration}";

        public IEnumerable<MessageHandler> CreateInstancesInEveryRoleFor<TMessage>(TMessage message, MessageHandlerContext context)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.Operation.IsSupported(Configuration.SupportedOperationTypes))
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
                    from interfaceType in Interfaces
                    let messageTypeOfInterface = GetMessageTypeOf(interfaceType)
                    where messageTypeOfInterface.IsInstanceOfType(message)
                    select CreateMessageHandler(message, context, interfaceType);
            }
            return Enumerable.Empty<MessageHandler>();
        }

        private static readonly ConcurrentDictionary<Type, Func<object, object, MessageHandlerContext, Type, Type, MessageHandler>> _MessageHandlerFactories =
            new ConcurrentDictionary<Type, Func<object, object, MessageHandlerContext, Type, Type, MessageHandler>>();

        private MessageHandler CreateMessageHandler<TMessage>(TMessage message, MessageHandlerContext context, Type interfaceType) =>
            _MessageHandlerFactories.GetOrAdd(interfaceType, CreateMessageHandlerFactory).Invoke(ResolveMessageHandler(context), message, context, Type, interfaceType);                                             
        
        private static Func<object, object, MessageHandlerContext, Type, Type, MessageHandler> CreateMessageHandlerFactory(Type interfaceType)
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
            // - return new MessageHandlerDecorator<object>(Resolve<ObjectHandler>(), message, context, typeof(ObjectHandler), typeof(IMessageHandler<object>));
            var messageType = GetMessageTypeOf(interfaceType);

            var handlerParameter = Expression.Parameter(typeof(object), "handler");
            var handlerOfExpectedType = Expression.Convert(handlerParameter, interfaceType);
            var messageParameter = Expression.Parameter(typeof(object), "message");
            var messageOfExpectedType = Expression.Convert(messageParameter, messageType);
            var contextParameter = Expression.Parameter(typeof(MessageHandlerContext), "context");
            var typeParameter = Expression.Parameter(typeof(Type), "type");
            var interfaceTypeParameter = Expression.Parameter(typeof(Type), "interfaceType");

            var decoratorTypeDefinition = typeof(MessageHandlerDecorator<>);
            var decoratorType = decoratorTypeDefinition.MakeGenericType(messageType);
            var decoratorConstructorParameters = new[] { interfaceType, messageType, typeof(MessageHandlerContext), typeof(Type), typeof(Type) };
            var decoratorConstructor = decoratorType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, decoratorConstructorParameters, null);
            var newDecoratorExpression = Expression.New(decoratorConstructor, handlerOfExpectedType, messageOfExpectedType, contextParameter, typeParameter, interfaceTypeParameter);
            var newMessageHandlerExpression = Expression.Lambda<Func<object, object, MessageHandlerContext, Type, Type, MessageHandler>>(
                Expression.Convert(newDecoratorExpression, typeof(MessageHandler)),
                handlerParameter,
                messageParameter,
                contextParameter,
                typeParameter,
                interfaceTypeParameter);

            return newMessageHandlerExpression.Compile();
        }

        private object ResolveMessageHandler(MicroProcessorContext context)
        {
            var messageHandler = context.ServiceProvider.GetService(Type);
            if (messageHandler == null)
            {
                throw NewCouldNotResolveMessageHandlerException(context.ServiceProvider.GetType(), Type);
            }
            return messageHandler;
        }

        private static Exception NewCouldNotResolveMessageHandlerException(Type serviceProviderType, Type messageHandlerType)
        {
            var messageFormat = ExceptionMessages.MessageHandlerClass_CouldNotResolveMessageHandler;
            var message = string.Format(messageFormat, serviceProviderType.FriendlyName(), messageHandlerType.FriendlyName());
            return new InvalidOperationException(message);
        }

        #region [====== FromTypes ======]

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _MessageHandlerTypeDefinition = typeof(IMessageHandler<>);

        public static IEnumerable<MessageHandlerClass> FromTypes(IEnumerable<Type> types)
        {      
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }
            foreach (var type in types.Distinct())
            {
                if (IsMessageHandlerClass(type, out var messageHandlerClass))
                {
                    yield return messageHandlerClass;
                }
            }            
        }

        internal static bool IsMessageHandlerClass(Type type, out MessageHandlerClass handler)
        {
            if (type.IsAbstract || !type.IsClass || type.ContainsGenericParameters)
            {
                handler = null;
                return false;
            }
            var interfaceTypes = GetMessageHandlerInterfaceTypesImplementedBy(type);
            if (interfaceTypes.Length == 0)
            {
                handler = null;
                return false;
            }
            handler = new MessageHandlerClass(type, interfaceTypes, DetermineMessageHandlerConfigurationOf(type));
            return true;
        }

        private static IMessageHandlerConfiguration DetermineMessageHandlerConfigurationOf(Type type)
        {
            if (TryGetMessageHandlerAttribute(type, out var configuration))
            {
                return configuration;
            }
            return MessageHandlerConfiguration.Default;
        }

        internal static bool TryGetMessageHandlerAttribute(Type classType, out IMessageHandlerConfiguration attribute)
        {
            var attributes = classType.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
            if (attributes.Length == 0)
            {
                attribute = null;
                return false;
            }
            attribute = attributes.Cast<MessageHandlerAttribute>().Single();
            return true;
        }

        private static Type[] GetMessageHandlerInterfaceTypesImplementedBy(Type classType) =>
            _MessageHandlerInterfaceTypes.GetOrAdd(classType, type => GetMessageHandlerInterfaceTypesImplementedByCore(type).ToArray());

        private static IEnumerable<Type> GetMessageHandlerInterfaceTypesImplementedByCore(Type classType) =>
            from interfaceType in classType.GetInterfaces()
            where IsMessageHandlerInterface(interfaceType)
            select interfaceType;

        private static bool IsMessageHandlerInterface(Type interfaceType)
        {
            if (interfaceType.IsGenericType)
            {
                return interfaceType.GetGenericTypeDefinition() == _MessageHandlerTypeDefinition;
            }
            return false;
        }

        #endregion

        internal static Type GetMessageTypeOf(Type interfaceType) =>
            interfaceType.GetGenericArguments()[0];
    }
}
