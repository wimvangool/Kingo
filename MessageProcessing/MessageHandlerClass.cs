using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YellowFlare.MessageProcessing
{
    internal sealed class MessageHandlerClass
    {        
        private readonly MessageHandlerFactory _container;
        private readonly Type _classType;
        private readonly Type[] _interfaceTypes;

        private MessageHandlerClass(MessageHandlerFactory container, Type classType, Type[] interfaceTypes)
        {
            _container = container;
            _classType = classType;
            _interfaceTypes = interfaceTypes;
        }  
      
        private IEnumerable<Type> InternalHandlerTypes
        {
            get { return _interfaceTypes.Where(type => type.GetGenericTypeDefinition() == _InternalTypeDefinition); }
        }

        private IEnumerable<Type> ExternalHandlerTypes
        {
            get { return _interfaceTypes.Where(type => type.GetGenericTypeDefinition() == _ExternalTypeDefinition); }
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
                    _container.RegisterWithPerResolveLifetime(_classType);
                    return;  
                 
                case InstanceLifetime.PerUnitOfWork:                    
                    _container.RegisterWithPerUnitOfWorkLifetime(_classType);
                    return;
                    
                case InstanceLifetime.Single:                    
                    _container.RegisterSingle(_classType);
                    return;
                    
                default:                    
                    throw NewInvalidLifetimeModeSpecifiedException(_classType, lifeTime);                    
            }
        }                       

        public IEnumerable<object> ResolveInEveryRoleForInternal(Type messageType)
        {
            return from interfaceType in InternalHandlerTypes
                   let messageTypeOfInterface = GetMessageTypeOf(interfaceType)
                   where messageTypeOfInterface.IsAssignableFrom(messageType)
                   let roleTypeDefinition = typeof(InternalMessageHandler<>)
                   let roleType = roleTypeDefinition.MakeGenericType(messageTypeOfInterface)
                   select ResolveInRoleFor(roleType);
        }

        public IEnumerable<object> ResolveInEveryRoleForExternal(Type messageType)
        {
            return from interfaceType in ExternalHandlerTypes
                   let messageTypeOfInterface = GetMessageTypeOf(interfaceType)
                   where messageTypeOfInterface.IsAssignableFrom(messageType)
                   let roleTypeDefinition = typeof(ExternalMessageHandler<>)
                   let roleType = roleTypeDefinition.MakeGenericType(messageTypeOfInterface)
                   select ResolveInRoleFor(roleType);
        }

        private object ResolveInRoleFor(Type roleType)
        {                            
            return Activator.CreateInstance(roleType, _container.Resolve(_classType));
        }

        private static readonly ConcurrentDictionary<Type, Type[]> _MessageHandlerInterfaceTypes = new ConcurrentDictionary<Type, Type[]>();
        private static readonly Type _InternalTypeDefinition = typeof(IInternalMessageHandler<>);
        private static readonly Type _ExternalTypeDefinition = typeof(IExternalMessageHandler<>);

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
                Type typeDefinition = interfaceType.GetGenericTypeDefinition();

                return
                    typeDefinition == _InternalTypeDefinition ||
                    typeDefinition == _ExternalTypeDefinition;
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
