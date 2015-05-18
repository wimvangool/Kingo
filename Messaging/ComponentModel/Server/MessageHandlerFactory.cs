using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented, represents a factory of all message-handlers that are used to handle the messages
    /// for the <see cref="MessageProcessor"/>.
    /// </summary>
    public abstract class MessageHandlerFactory
    {
        [DebuggerDisplay("Count = {_messageHandlers.Count}")]
        private readonly List<MessageHandlerClass> _messageHandlers;
        private readonly UnitOfWorkCache _unitOfWorkCache;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory" /> class.
        /// </summary>        
        protected MessageHandlerFactory()
        {
            _messageHandlers = new List<MessageHandlerClass>();    
            _unitOfWorkCache = new UnitOfWorkCache();
        }        

        /// <summary>
        /// Returns the cache that can be used to store dependencies with a lifetime of <see cref="InstanceLifetime.PerUnitOfWork" />.
        /// </summary>
        protected IDependencyCache UnitOfWorkCache
        {
            get { return _unitOfWorkCache; }
        }

        /// <summary>
        /// Returns the number of classes implementing the <see cref="IMessageHandler{T}" /> interface have been registered in this factory.
        /// </summary>
        public int MessageHandlerCount
        {
            get { return _messageHandlers.Count; }
        }        

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} MessageHandler(s) Registered", _messageHandlers.Count);
        }

        #region [====== MessageHandlers ======]

        /// <summary>
        /// Registers all handlers found in the specified <paramref name="assemblies"/> with this factory.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public void RegisterMessageHandlers(AssemblySet assemblies)
        {
            RegisterMessageHandlers(assemblies, null);
        }

        /// <summary>
        /// Registers all handlers found in the specified <paramref name="assemblies"/> and satisfy
        /// the specified <paramref name="typeSelector"/> with this factory.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search.</param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes from the
        /// complete set found in the specified <paramref name="assemblies"/>.
        /// </param>
        /// <param name="configurationPerType">
        /// A mapping from certain <see cref="IMessageHandler{T}" /> classes to their own respective configurations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public void RegisterMessageHandlers(AssemblySet assemblies, Func<Type, bool> typeSelector, MessageHandlerToConfigurationMapping configurationPerType = null)
        {
            foreach (var messageHandler in MessageHandlerClass.RegisterMessageHandlers(this, assemblies, typeSelector, configurationPerType))
            {
                _messageHandlers.Add(messageHandler);
            }          
        }

        /// <inheritdoc />
        internal IEnumerable<MessageHandlerInstance<TMessage>> CreateMessageHandlersFor<TMessage>(TMessage message, MessageSources source) where TMessage : class
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return from handlerClass in _messageHandlers
                   let handlers = handlerClass.CreateInstancesInEveryRoleFor(message, source)
                   from handler in handlers
                   select handler;
        }        

        /// <summary>
        /// Create an instance of the requested message handler type.
        /// </summary>
        /// <param name="type">Type to create.</param>
        /// <returns>An instance of the requested message handler type.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>                
        protected internal abstract object CreateMessageHandler(Type type);

        #endregion

        #region [====== Dependencies ======]

        /// <summary>
        /// Registers a set of types that are found in the InterfaceLayer and Data Access Layer as dependencies
        /// and are marked with the <see cref="DependencyAttribute"/>.
        /// </summary>      
        /// <param name="assemblies">A set of assemblies to search.</param>  
        /// <param name="configurationPerType">
        /// A mapping from certain dependencies to their own respective configurations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public void RegisterDependencies(AssemblySet assemblies, DependencyToConfigurationMapping configurationPerType = null)
        {
            RegisterDependencies(assemblies, null, configurationPerType);
        }

        /// <summary>
        /// Registers a set of types that are found in the InterfaceLayer and Data Access Layer as dependencies
        /// and are marked with the <see cref="DependencyAttribute"/>.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search.</param>
        /// <param name="concreteTypePredicate">A predicate that identifies which types should be registered as dependencies.</param>    
        /// <param name="configurationPerType">
        /// A mapping from certain dependencies to their own respective configurations.
        /// </param>   
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        public void RegisterDependencies(AssemblySet assemblies, Func<Type, bool> concreteTypePredicate, DependencyToConfigurationMapping configurationPerType = null)
        {
            DependencyClass.RegisterDependencies(this, assemblies, concreteTypePredicate, configurationPerType);
        }        

        /// <summary>
        /// Registers a set of types that are found in the InterfaceLayer and Data Access Layer as dependencies that are
        /// implementations of certain abstract types (interfaces or abstract classes) that are found in the Application Layer
        /// or Domain Layer and are marked with the <see cref="DependencyAttribute"/>.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search.</param>
        /// <param name="concreteTypePredicate">A predicate that identifies which concrete types should be registered as dependencies.</param>
        /// <param name="abstractTypePredicate">A predicate that identifies which abstract types should be registered as dependencies.</param>
        /// <param name="configurationPerType">
        /// A mapping from certain dependencies to their own respective configurations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more types specified by <paramref name="abstractTypePredicate"/> match with multiple concrete implementations.
        /// </exception>
        /// <remarks>
        /// By default, all types will be registered with a <see cref="InstanceLifetime.PerUnitOfWork" /> lifetime.
        /// </remarks>
        public void RegisterDependencies(AssemblySet assemblies, Func<Type, bool> concreteTypePredicate, Func<Type, bool> abstractTypePredicate, DependencyToConfigurationMapping configurationPerType = null)
        {
            DependencyClass.RegisterDependencies(this, assemblies, concreteTypePredicate, abstractTypePredicate, configurationPerType);
        }        

        #endregion

        #region [====== Type Registration ======]

        /// <summary>
        /// Registers a certain type of which a new instance is created each time it is resolved.
        /// </summary>
        /// <param name="concreteType">A concrete type.</param>        
        protected internal abstract void RegisterWithPerResolveLifetime(Type concreteType);

        /// <summary>
        /// Registers a certain type for a specified <paramref name="abstractType"/> of which a new instance is
        /// created each time it is resolved, for a certain abstract type.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements <paramref name="abstractType"/>.</param>
        /// <param name="abstractType">An abstract type.</param>
        protected internal abstract void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType);

        /// <summary>
        /// Registers a certain type to resolve with a lifetime that spans a single Unit of Work.
        /// </summary>
        /// <param name="concreteType">A concrete type.</param>        
        protected internal abstract void RegisterWithPerUnitOfWorkLifetime(Type concreteType);        

        /// <summary>
        /// Registers a certain type to resolve for a specified <paramref name="abstractType"/>
        /// with a lifetime that spans a single Unit of Work.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements <paramref name="abstractType"/>.</param>
        /// <param name="abstractType">An abstract type.</param>       
        protected internal abstract void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType);

        /// <summary>
        /// Registers a certain type with a lifetime that reflects that of a singleton.
        /// </summary>
        /// <param name="concreteType">A concrete type.</param>               
        protected internal abstract void RegisterSingleton(Type concreteType);

        /// <summary>
        /// Registers a certain type with a lifetime that reflects that of a singleton for a certain <paramref name="abstractType"/>.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements <paramref name="abstractType"/>.</param>
        /// <param name="abstractType">An abstract type.</param>  
        protected internal abstract void RegisterSingleton(Type concreteType, Type abstractType);

        #endregion
    }
}
