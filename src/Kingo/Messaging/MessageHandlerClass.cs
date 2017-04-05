using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a class that is registered as a <see cref="IMessageHandler{T}" />.
    /// </summary>
    public sealed class MessageHandlerClass
    {
        #region [====== Instance ======]

        private sealed class Instance<TMessage> : IMessageHandler<TMessage>
        {
            private readonly IMessageHandler<TMessage> _handler;

            public Instance(IMessageHandler<TMessage> handler)
            {
                _handler = handler;
            }

            public Task HandleAsync(TMessage message, IMicroProcessorContext context) =>
                _handler.HandleAsync(message, context);
        }

        #endregion
        
        private readonly Type _type;
        private readonly Type[] _interfaces;
        private readonly IMessageHandlerConfiguration _configuration;

        private MessageHandlerClass(Type type, Type[] interfaces, IMessageHandlerConfiguration configuration)
        {            
            _type = type;
            _interfaces = interfaces;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns the type of this message handler class.
        /// </summary>
        public Type Type =>
            _type;

        /// <summary>
        /// Returns all <see cref="IMessageHandler{T}" /> interface types that are implemented by this class.
        /// </summary>
        public IEnumerable<Type> Interfaces =>
            _interfaces;

        /// <summary>
        /// Returns the configuration of this message handler class.
        /// </summary>
        public IMessageHandlerConfiguration Configuration =>
            _configuration;

        private MessageHandlerClass RegisterIn(MessageHandlerFactory factory)
        {
            factory.Register(_type, _configuration.Lifetime);
            return this;
        }

        internal IEnumerable<MessageHandler<TMessage>> CreateInstancesInEveryRoleFor<TMessage>(MessageHandlerFactory factory, MessageHandlerContext context, TMessage message)
        {
            if (IsAcceptedSource(_configuration.Sources, context.Messages.Current.Source))
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
                       select CreateMessageHandlerInstanceFor<TMessage>(factory, context, interfaceType, messageTypeOfInterface);
            }
            return Enumerable.Empty<MessageHandler<TMessage>>();
        }

        private MessageHandler<TMessage> CreateMessageHandlerInstanceFor<TMessage>(MessageHandlerFactory factory, MessageHandlerContext context, Type interfaceType, Type messageTypeOfInterface)
        {
            // In order to support message handlers with contravariant IMessageHandler<TMessage>-implementations, the resolved instance is wrapped
            // inside an Instance<TActual> where TActual is the type of the generic type parameter of the specified interface. This ensures that
            // the correct interface method of the handler is called at runtime.
            //
            // For example, suppose a message handler is declared as follows...
            //
            // public sealed class ObjectHandler : IMessageHandler<object>
            // {
            //     public Task<IMessageStream> HandleAsync(object message, IMicroProcessorContext context) 
            //     {
            //         ...
            //     }    
            // }
            //
            // ...and this method is called with a TMessage of type 'string', then the code will do something allong the following lines:
            //
            // - var handler = Resolve<ObjectHandler>();
            // - var objectHandler = new Instance<object>(handler);
            // - var stringHandler = (IMessageHandler<string>) objectHandler;
            //
            // - return new MessageHandlerDecorator<string>(stringHandler, typeof(ObjectHandler), typeof(IMessageHandler<object>));
            var handler = factory.Resolve(_type);
            var instanceTypeDefinition = typeof(Instance<>);
            var instanceType = instanceTypeDefinition.MakeGenericType(messageTypeOfInterface);
            var instanceConstructor = instanceType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { interfaceType }, null);
            var instance = (IMessageHandler<TMessage>) instanceConstructor.Invoke(new[] { handler });

            return new MessageHandlerDecorator<TMessage>(context, instance, _type, interfaceType);
        }

        private static bool IsAcceptedSource(MessageSources sources, MessageSources source)
        {
            return (sources & source) == source;
        }

        public override string ToString() =>
            $"{_type.Name} ({_interfaces.Length} interface(s) implemented) - Configuration = {_configuration}";

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _MessageHandlerTypeDefinition = typeof(IMessageHandler<>);

        internal static IEnumerable<MessageHandlerClass> RegisterMessageHandlers(MessageHandlerFactory factory, IEnumerable<Type> types)
        {            
            var messageHandlers = new List<MessageHandlerClass>();            

            foreach (var type in types)
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

        private static IEnumerable<Type> GetMessageHandlerInterfaceTypesImplementedByCore(Type classType)
        {
            return
                from interfaceType in classType.GetInterfaces()
                where IsMessageHandlerInterface(interfaceType)
                select interfaceType;
        }

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
