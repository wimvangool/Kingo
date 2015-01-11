using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerClass
    {        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly MessageHandlerFactory _factory;        
        private readonly Type _classType;
        private readonly Type[] _interfaceTypes;        

        private MessageHandlerClass(MessageHandlerFactory factory, Type classType, Type[] interfaceTypes)
        {
            _factory = factory;
            _classType = classType;
            _interfaceTypes = interfaceTypes;
        }                     

        private void Register(InstanceLifetime lifetime)
        {
            switch (lifetime)
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
                    throw NewInvalidLifetimeModeSpecifiedException(_classType, lifetime);                    
            }
        }                       

        public IEnumerable<IMessageHandler<TMessage>> CreateInstancesInEveryRoleFor<TMessage>(TMessage message) where TMessage : class
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
                   select (IMessageHandler<TMessage>) _factory.CreateMessageHandler(_classType);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1} interface(s) implemented)", _classType.Name, _interfaceTypes.Length);
        }

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _MessageHandlerTypeDefinition = typeof(IMessageHandler<>);                       

        public static bool TryRegisterIn(MessageHandlerFactory container, Type type, Func<Type, bool> predicate, out MessageHandlerClass handler)
        {
            InstanceLifetime lifetime;

            if (type.IsAbstract || !type.IsClass || type.ContainsGenericParameters || !HasMessageHandlerAttribute(type, out lifetime) || !SatisfiesPredicate(type, predicate))
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
            handler = new MessageHandlerClass(container, type, interfaceTypes);
            handler.Register(lifetime);
            return true;
        }

        private static bool HasMessageHandlerAttribute(Type classType, out InstanceLifetime lifetime)
        {
            var attributes = classType.GetCustomAttributes(typeof(MessageHandlerAttribute), true);
            if (attributes.Length == 0)
            {
                lifetime = InstanceLifetime.PerResolve;
                return false;
            }
            lifetime = attributes.Cast<MessageHandlerAttribute>().Single().Lifetime;
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
