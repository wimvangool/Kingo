using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerClass
    {        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MessageHandlerFactory _factory;        
        private readonly Type _classType;
        private readonly Type[] _interfaceTypes;
        private readonly IMessageHandlerConfiguration _configuration;

        private MessageHandlerClass(MessageHandlerFactory factory, Type classType, Type[] interfaceTypes, IMessageHandlerConfiguration configuration)
        {
            _factory = factory;
            _classType = classType;
            _interfaceTypes = interfaceTypes;
            _configuration = configuration;
        }                     

        private void Register()
        {
            switch (_configuration.Lifetime)
            {
                case InstanceLifetime.PerResolve:                    
                    _factory.RegisterWithPerResolveLifetime(_classType);
                    return;  
                 
                case InstanceLifetime.PerUnitOfWork:                    
                    _factory.RegisterWithPerUnitOfWorkLifetime(_classType);
                    return;
                    
                case InstanceLifetime.Singleton:                    
                    _factory.RegisterSingleton(_classType);
                    return;
                    
                default:                    
                    throw NewInvalidLifetimeModeSpecifiedException(_classType, _configuration.Lifetime);                    
            }
        }                       

        internal IEnumerable<MessageHandlerInstance<TMessage>> CreateInstancesInEveryRoleFor<TMessage>(TMessage message, MessageSources source) where TMessage : class
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
                return from interfaceType in _interfaceTypes
                       let messageTypeOfInterface = GetMessageTypeOf(interfaceType)
                       where messageTypeOfInterface.IsInstanceOfType(message)
                       select CreateMessageHandlerInstanceFor<TMessage>(interfaceType, messageTypeOfInterface);                       
            }
            return Enumerable.Empty<MessageHandlerInstance<TMessage>>();
        }

        private MessageHandlerInstance<TMessage> CreateMessageHandlerInstanceFor<TMessage>(Type interfaceType, Type messageTypeOfInterface) where TMessage : class
        {            
            var messageHandler = _factory.CreateMessageHandler(_classType);
            var decoratorTypeDefinition = typeof(MessageHandlerDecorator<>);
            var decoratorType = decoratorTypeDefinition.MakeGenericType(messageTypeOfInterface);
            var decoratorConstructor = decoratorType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new [] { interfaceType }, null);
            var decorator = (IMessageHandler<TMessage>) decoratorConstructor.Invoke(new [] { messageHandler });

            return new MessageHandlerInstance<TMessage>(decorator, _classType, interfaceType);
        }

        private static bool IsAcceptedSource(MessageSources sources, MessageSources source)
        {
            return (sources & source) == source;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1} interface(s) implemented) - Configuration = {2}", _classType.Name, _interfaceTypes.Length, _configuration);
        }

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _MessageHandlerTypeDefinition = typeof(IMessageHandler<>); 
        
        internal static IEnumerable<MessageHandlerClass> RegisterMessageHandlers(MessageHandlerFactory factory, AssemblySet assemblies, Func<Type, bool> typeSelector, MessageHandlerToConfigurationMapping configurationPerType)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }
            var messageHandlers = new List<MessageHandlerClass>();

            foreach (var type in assemblies.GetTypes())
            {
                MessageHandlerClass handler;

                if (TryRegisterIn(factory, type, typeSelector, configurationPerType, out handler))
                {
                    messageHandlers.Add(handler);
                }
            }
            return messageHandlers;
        }

        private static bool TryRegisterIn(MessageHandlerFactory container, Type type, Func<Type, bool> predicate, MessageHandlerToConfigurationMapping configurationPerType, out MessageHandlerClass handler)
        {            
            if (type.IsAbstract || !type.IsClass || type.ContainsGenericParameters || !SatisfiesPredicate(type, predicate))
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
            var configuration = DetermineMessageHandlerConfiguration(type, configurationPerType);

            handler = new MessageHandlerClass(container, type, interfaceTypes, configuration);
            handler.Register();
            return true;            
        }

        private static IMessageHandlerConfiguration DetermineMessageHandlerConfiguration(Type type, MessageHandlerToConfigurationMapping configurationPerType)
        {
            IMessageHandlerConfiguration configuration;

            if (configurationPerType == null)
            {
                if (TryGetMessageHandlerAttribute(type, out configuration))
                {
                    return configuration;
                }
                return MessageHandlerConfiguration.Default;
            }
            if (configurationPerType.TryGetValue(type, out configuration))
            {
                return configuration;
            }
            if (TryGetMessageHandlerAttribute(type, out configuration))
            {
                return configuration;
            }
            return configurationPerType.DefaultConfiguration;
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

        private static bool SatisfiesPredicate(Type type, Func<Type, bool> predicate)
        {
            return predicate == null || predicate.Invoke(type);
        }

        public static bool IsMessageHandler(Type classType)
        {
            return GetMessageHandlerInterfaceTypesImplementedBy(classType).Length > 0;
        }

        private static Type[] GetMessageHandlerInterfaceTypesImplementedBy(Type classType)
        {
            return _MessageHandlerInterfaceTypes.GetOrAdd(classType, type => GetMessageHandlerInterfaceTypesImplementedByCore(type).ToArray());
        }

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

        private static Type GetMessageTypeOf(Type interfaceType)
        {
            return interfaceType.GetGenericArguments()[0];
        }        

        internal static Exception NewInvalidLifetimeModeSpecifiedException(Type classType, InstanceLifetime lifeTime)
        {
            var messageFormat = ExceptionMessages.MessageHandlerClass_InvalidInstanceLifetimeMode;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, classType, lifeTime);
            return new InvalidOperationException(message);
        }
    }
}
