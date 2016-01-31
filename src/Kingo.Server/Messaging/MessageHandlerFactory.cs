using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented, represents a factory of all message-handlers that are used to handle the messages
    /// for the <see cref="MessageProcessor"/>.
    /// </summary>
    public abstract class MessageHandlerFactory
    {
        [DebuggerDisplay("Count = {_messageHandlers.Count}")]
        private readonly List<MessageHandlerClass> _messageHandlers;        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory" /> class.
        /// </summary>        
        protected MessageHandlerFactory()
        {
            _messageHandlers = new List<MessageHandlerClass>();                
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
        /// Registers all handlers found in the <see cref="LayerConfiguration.ApplicationLayer" /> and
        /// <see cref="LayerConfiguration.DataAccessLayer" /> of the specified <paramref name="layers"/>
        /// and satisfy the specified <paramref name="typeSelector"/> with this factory.
        /// </summary>
        /// <param name="layers">A collection of application layers to scan.</param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes.
        /// </param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a <see cref="IMessageHandlerWrapper"/> based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layers"/> is <c>null</c>.
        /// </exception>        
        public void RegisterMessageHandlers(LayerConfiguration layers, Predicate<Type> typeSelector = null, Func<Type, IMessageHandlerConfiguration> configurationFactory = null)
        {            
            RegisterMessageHandlers(JoinMessageHandlerLayers(layers), typeSelector, configurationFactory);
        }   

        /// <summary>
        /// Registers all handlers found in the specified collection of <paramref name="types"/> and satisfy
        /// the specified <paramref name="typeSelector"/> with this factory.
        /// </summary>
        /// <param name="types">A collection of types that will be scanned.</param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes.
        /// </param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a <see cref="IMessageHandlerWrapper"/> based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>        
        public void RegisterMessageHandlers(IEnumerable<Type> types, Predicate<Type> typeSelector = null, Func<Type, IMessageHandlerConfiguration> configurationFactory = null)
        {
            foreach (var messageHandler in MessageHandlerClass.RegisterMessageHandlers(this, types, typeSelector, configurationFactory))
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
        protected internal abstract object Resolve(Type type);

        private static Layer JoinMessageHandlerLayers(LayerConfiguration layers)
        {
            if (layers == null)
            {
                throw new ArgumentNullException("layers");
            }
            return layers.ApplicationLayer + layers.DataAccessLayer;
        }

        #endregion

        #region [====== Repositories ======]

        /// <summary>
        /// Registers all repository interface types that are found in the <see cref="LayerConfiguration.DomainLayer" /> and
        /// <see cref="LayerConfiguration.DataAccessLayer" /> of the specified <paramref name="layers"/>,
        /// along with with their implementations.
        /// </summary>
        /// <param name="layers">A collection of types to scan.</param>
        /// <param name="postfix">The postfix that every repository interface ends with. Default is <c>Repository</c>.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="layers"/> is <c>null</c>.
        /// </exception>        
        public void RegisterRepositories(LayerConfiguration layers, string postfix = null, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            RegisterRepositories(JoinRepositoryLayers(layers), postfix, configurationFactory);
        }

        /// <summary>
        /// Registers all repository interface types that are found inside the specified collection of <paramref name="types"/>
        /// along with their implementations.
        /// </summary>
        /// <param name="types">A collection of types to scan.</param>
        /// <param name="postfix">The postfix that every repository interface ends with. Default is <c>Repository</c>.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>        
        public void RegisterRepositories(IEnumerable<Type> types, string postfix = null, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            RegisterDependencies(types, null, type => IsRepositoryInterface(type, postfix), configurationFactory);
        }

        private static bool IsRepositoryInterface(Type type, string postfix)
        {
            return type.Name.EndsWith(postfix ?? "Repository");
        }

        private static Layer JoinRepositoryLayers(LayerConfiguration layers)
        {
            if (layers == null)
            {
                throw new ArgumentNullException("layers");
            }
            return layers.DomainLayer + layers.DataAccessLayer;
        }

        #endregion

        #region [====== Dependencies ======]

        /// <summary>
        /// Registers a set of types that are found in the specified assembies
        /// and are selected by the specified <paramref name="concreteTypePredicate"/>.
        /// </summary>
        /// <param name="types">A collection of types to scan.</param>
        /// <param name="concreteTypePredicate">A predicate that identifies which types should be registered as dependencies.</param>    
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> or <paramref name="concreteTypePredicate"/> is <c>null</c>.
        /// </exception>        
        public void RegisterDependencies(IEnumerable<Type> types, Predicate<Type> concreteTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            DependencyClass.RegisterDependencies(this, types, concreteTypePredicate, configurationFactory);
        }        

        /// <summary>
        /// Registers a set of abstract types that are mapped to a set of concrete types. Both sets are found in the specified assembies
        /// and are selected by the specified <paramref name="concreteTypePredicate"/> and <paramref name="abstractTypePredicate"/>.
        /// </summary>
        /// <param name="types">A collection of types to scan.</param>
        /// <param name="concreteTypePredicate">A predicate that identifies which concrete types should be registered as dependencies (optional).</param>
        /// <param name="abstractTypePredicate">A predicate that identifies which abstract types should be registered as dependencies.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> or <paramref name="abstractTypePredicate"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more types specified by <paramref name="abstractTypePredicate"/> match with multiple concrete implementations.
        /// </exception>                
        public void RegisterDependencies(IEnumerable<Type> types, Predicate<Type> concreteTypePredicate, Predicate<Type> abstractTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            DependencyClass.RegisterDependencies(this, types, concreteTypePredicate, abstractTypePredicate, configurationFactory);
        }        

        #endregion

        #region [====== Type Registration ======]

        /// <summary>
        /// Registers a certain type for a specified <paramref name="abstractType"/> of which a new instance is
        /// created each time it is resolved, for a certain abstract type.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements or is a sub-class of <paramref name="abstractType"/>, if specified.</param>
        /// <param name="abstractType">An abstract type.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="concreteType"/> is <c>null</c>.
        /// </exception>
        public abstract void RegisterWithPerResolveLifetime(Type concreteType, Type abstractType = null);                

        /// <summary>
        /// Registers a certain type to resolve for a specified <paramref name="abstractType"/>
        /// with a lifetime that spans a single Unit of Work.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements or is a sub-class of <paramref name="abstractType"/>, if specified.</param>
        /// <param name="abstractType">An abstract type.</param>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="concreteType"/> is <c>null</c>.
        /// </exception>     
        public abstract void RegisterWithPerUnitOfWorkLifetime(Type concreteType, Type abstractType = null);        

        /// <summary>
        /// Registers a certain type with a lifetime that reflects that of a singleton for a certain <paramref name="abstractType"/>.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements or is a sub-class of <paramref name="abstractType"/>, if specified.</param>
        /// <param name="abstractType">An abstract type.</param> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="concreteType"/> is <c>null</c>.
        /// </exception> 
        public abstract void RegisterSingleton(Type concreteType, Type abstractType = null);

        /// <summary>
        /// Registers a certain type with a lifetime that reflects that of a singleton for a certain <paramref name="abstractType"/>.
        /// </summary>
        /// <param name="concreteType">A concrete type that implements or is a sub-class of <paramref name="abstractType"/>, if specified.</param>
        /// <param name="abstractType">An abstract type.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="concreteType"/> is <c>null</c>.
        /// </exception> 
        public abstract void RegisterSingleton(object concreteType, Type abstractType = null);

        #endregion


    }
}
