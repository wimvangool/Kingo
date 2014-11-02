using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Globalization;
using System.Linq;

namespace System.ComponentModel.Server
{
    internal sealed class MessageHandlerClass
    {        
        private readonly MessageHandlerFactory _factory;
        private readonly Type _classType;
        private readonly Type[] _interfaceTypes;

        private MessageHandlerClass(MessageHandlerFactory factory, Type classType, Type[] interfaceTypes)
        {
            _factory = factory;
            _classType = classType;
            _interfaceTypes = interfaceTypes;
        }                     

        private void Register()
        {
            var lifeTimeAttribute = _classType
                .GetCustomAttributes(typeof(InstanceLifetimeAttribute), true)
                .Cast<InstanceLifetimeAttribute>()
                .SingleOrDefault();

            var lifeTime = lifeTimeAttribute == null ? InstanceLifetime.PerResolve : lifeTimeAttribute.Lifetime;

            switch (lifeTime)
            {
                case InstanceLifetime.PerResolve:                    
                    _factory.RegisterWithPerResolveLifetime(_classType);
                    return;  
                 
                case InstanceLifetime.PerUnitOfWork:                    
                    _factory.RegisterWithPerUnitOfWorkLifetime(_classType);
                    return;
                    
                case InstanceLifetime.Single:                    
                    _factory.RegisterSingle(_classType);
                    return;
                    
                default:                    
                    throw NewInvalidLifetimeModeSpecifiedException(_classType, lifeTime);                    
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

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _MessageHandlerTypeDefinition = typeof(IMessageHandler<>);                       

        public static bool TryRegisterIn(MessageHandlerFactory container, Type type, Func<Type, bool> predicate, out MessageHandlerClass handler)
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
            handler = new MessageHandlerClass(container, type, interfaceTypes);
            handler.Register();
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

        private static Exception NewInvalidLifetimeModeSpecifiedException(Type classType, InstanceLifetime lifeTime)
        {
            var messageFormat = ExceptionMessages.MessageHandlerClass_InvalidInstanceLifetimeMode;
            var message = string.Format(CultureInfo.CurrentCulture, messageFormat, classType, lifeTime);
            return new InvalidOperationException(message);
        }
    }
}
