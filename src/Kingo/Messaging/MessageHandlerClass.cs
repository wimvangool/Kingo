using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a class that is registered as a <see cref="IMessageHandler{T}" />.
    /// </summary>
    public sealed class MessageHandlerClass
    {                
        private readonly Type _type;
        private readonly Type[] _interfaces;
        private readonly IMessageHandlerConfiguration _configuration;
        private readonly ConcurrentDictionary<Type, Func<object, object, Type, Type, MessageHandler>> _messageHandlerFactories;

        private MessageHandlerClass(Type type, Type[] interfaces, IMessageHandlerConfiguration configuration)
        {            
            _type = type;
            _interfaces = interfaces;
            _configuration = configuration;
            _messageHandlerFactories = new ConcurrentDictionary<Type, Func<object, object, Type, Type, MessageHandler>>();
        }        

        private MessageHandlerClass RegisterIn(MessageHandlerFactory factory)
        {
            factory.Register(_type, _configuration.Lifetime);
            return this;
        }

        internal IEnumerable<MessageHandler> CreateInstancesInEveryRoleFor<TMessage>(MessageHandlerFactory factory, MessageSources source, TMessage message)
        {
            if (IsAcceptedSource(_configuration.Sources, source))
            {
                // This LINQ construct first selects all message handler interface definitions that are compatible with
                // the specified message. Then it will dynamically create the correct message handler type for each match
                // and return it.
                //
                // Example:
                //   When the class implements both IMessageHandler<object> and IMessageHandler<SomeMessage>, then if
                //   TMessage is of type SomeMessage, two message-handler instances are returned, one for each
                //   implementation.
                return from interfaceType in _interfaces
                       let messageTypeOfInterface = GetMessageTypeOf(interfaceType)
                       where messageTypeOfInterface.IsInstanceOfType(message)
                       select CreateMessageHandlerInstance(factory, message, interfaceType, messageTypeOfInterface);
            }
            return Enumerable.Empty<MessageHandler>();
        }        

        private MessageHandler CreateMessageHandlerInstance<TMessage>(MessageHandlerFactory factory, TMessage message, Type interfaceType, Type messageTypeOfInterface)
        {
            // In order to support message handlers with contravariant IMessageHandler<TMessage>-implementations, the resolved instance is wrapped
            // inside an Instance<TActual> where TActual is the type of the generic type parameter of the specified interface. This ensures that
            // the correct interface method of the handler is called at runtime.
            //
            // For example, suppose a message handler is declared as follows...
            //
            // public sealed class ObjectHandler : IMessageHandler<object>
            // {
            //     public Task HandleAsync(object message, IMicroProcessorContext context) 
            //     {
            //         ...
            //     }    
            // }
            //
            // ...and this method is called with a TMessage of type 'string', then the code will do something allong the following lines:
            //            
            // - return new MessageHandlerDecorator<object>(Resolve<ObjectHandler>(), "abc", typeof(ObjectHandler), typeof(IMessageHandler<object>));                                    
            return _messageHandlerFactories.GetOrAdd(interfaceType, t =>
            {
                var handlerParameter = Expression.Parameter(typeof(object), "handler");
                var handlerOfExpectedType = Expression.Convert(handlerParameter, interfaceType);
                var messageParameter = Expression.Parameter(typeof(object), "message");
                var messageOfExpectedType = Expression.Convert(messageParameter, messageTypeOfInterface);
                var typeParameter = Expression.Parameter(typeof(Type), "type");
                var interfaceTypeParameter = Expression.Parameter(typeof(Type), "interfaceType");

                var decoratorTypeDefinition = typeof(MessageHandlerDecorator<>);
                var decoratorType = decoratorTypeDefinition.MakeGenericType(messageTypeOfInterface);
                var decoratorConstructorParameters = new[] { interfaceType, messageTypeOfInterface, typeof(Type), typeof(Type) };
                var decoratorConstructor = decoratorType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, decoratorConstructorParameters, null);
                var newDecoratorExpression = Expression.New(decoratorConstructor, handlerOfExpectedType, messageOfExpectedType, typeParameter, interfaceTypeParameter);                
                var newMessageHandlerExpression = Expression.Lambda<Func<object, object, Type, Type, MessageHandler>>(
                    Expression.Convert(newDecoratorExpression, typeof(MessageHandler)),
                    handlerParameter,
                    messageParameter,
                    typeParameter,
                    interfaceTypeParameter);

                return newMessageHandlerExpression.Compile();                
            }).Invoke(factory.Resolve(_type), message, _type, interfaceType);                                   
        }

        private static bool IsAcceptedSource(MessageSources sources, MessageSources source) =>
             (sources & source) == source;

        /// <inheritdoc />
        public override string ToString() =>
            $"{_type.Name} ({_interfaces.Length} interface(s) implemented) - Configuration = {_configuration}";

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _MessageHandlerTypeDefinition = typeof(IMessageHandler<>);

        internal static IEnumerable<MessageHandlerClass> RegisterMessageHandlers(MessageHandlerFactory factory, IEnumerable<Type> types)
        {            
            var messageHandlers = new List<MessageHandlerClass>();            

            foreach (var type in types.Distinct())
            {
                MessageHandlerClass handler;

                if (TryRegisterIn(factory, type, out handler))
                {
                    messageHandlers.Add(handler);
                }
            }
            return messageHandlers;
        }

        private static bool TryRegisterIn(MessageHandlerFactory factory, Type type, out MessageHandlerClass handler)
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
            handler = new MessageHandlerClass(type, interfaceTypes, DetermineMessageHandlerConfigurationOf(type)).RegisterIn(factory);            
            return true;
        }

        private static IMessageHandlerConfiguration DetermineMessageHandlerConfigurationOf(Type type)
        {
            IMessageHandlerConfiguration configuration;

            if (TryGetMessageHandlerAttribute(type, out configuration))
            {
                return configuration;
            }
            return MessageHandlerConfiguration.Default;
        }

        private static bool TryGetMessageHandlerAttribute(Type classType, out IMessageHandlerConfiguration attribute)
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

        private static IEnumerable<Type> GetMessageHandlerInterfaceTypesImplementedByCore(Type classType) => from interfaceType in classType.GetInterfaces()
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

        private static Type GetMessageTypeOf(Type interfaceType) =>
            interfaceType.GetGenericArguments()[0];        
    }
}
