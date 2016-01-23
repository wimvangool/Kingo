using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;

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
        /// Registers all handlers that are found in the assemblies located in the current directory and
        /// filtered by the specified <paramref name="searchPattern"/>.
        /// </summary>        
        /// <param name="searchPattern">A search pattern that is used to locate a set of assemblies.</param>
        /// <param name="searchOption">
        /// Indicates where the assemblies should be located.
        /// </param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes.
        /// </param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a <see cref="IMessageHandlerWrapper"/> based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="searchOption"/> is not a valid option.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterMessageHandlers(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly, Predicate<Type> typeSelector = null, Func<Type, IMessageHandlerConfiguration> configurationFactory = null)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }            
            RegisterMessageHandlers(new [] { searchPattern }, searchOption, typeSelector, configurationFactory);
        }

        /// <summary>
        /// Registers all handlers that are found in the assemblies located in the current directory and
        /// filtered by the specified search patterns.
        /// </summary>        
        /// <param name="searchPatternA">First search pattern that is used to locate a set of assemblies.</param>
        /// <param name="searchPatternB">Second search pattern that is used to locate a set of assemblies.</param>
        /// <param name="searchOption">
        /// Indicates where the assemblies should be located.
        /// </param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes.
        /// </param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a <see cref="IMessageHandlerWrapper"/> based on its type.
        /// </param>        
        /// <exception cref="ArgumentException">
        /// <paramref name="searchOption"/> is not a valid option.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterMessageHandlers(string searchPatternA, string searchPatternB, SearchOption searchOption = SearchOption.TopDirectoryOnly, Predicate<Type> typeSelector = null, Func<Type, IMessageHandlerConfiguration> configurationFactory = null)
        {            
            RegisterMessageHandlers(new[] { searchPatternA, searchPatternB }, searchOption, typeSelector, configurationFactory);
        } 

        /// <summary>
        /// Registers all handlers that are found in the assemblies located in the current directory and
        /// filtered by the specified <paramref name="searchPatterns"/>.
        /// </summary>        
        /// <param name="searchPatterns">A collection of search pattern that are used to locate multiple sets of assemblies.</param>
        /// <param name="searchOption">
        /// Indicates where the assemblies should be located.
        /// </param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes.
        /// </param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a <see cref="IMessageHandlerWrapper"/> based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPatterns"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="searchOption"/> is not a valid option.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterMessageHandlers(IEnumerable<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly, Predicate<Type> typeSelector = null, Func<Type, IMessageHandlerConfiguration> configurationFactory = null)
        {                        
            RegisterMessageHandlers(AssemblySet.FromCurrentDirectory(searchPatterns, searchOption), typeSelector, configurationFactory);
        }                

        /// <summary>
        /// Registers all handlers found in the specified <paramref name="assemblies"/> and satisfy
        /// the specified <paramref name="typeSelector"/> with this factory.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search through.</param>
        /// <param name="typeSelector">
        /// Selector that is used to select specific <see cref="IMessageHandler{T}" /> classes.
        /// </param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a <see cref="IMessageHandlerWrapper"/> based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterMessageHandlers(AssemblySet assemblies, Predicate<Type> typeSelector = null, Func<Type, IMessageHandlerConfiguration> configurationFactory = null)
        {
            foreach (var messageHandler in MessageHandlerClass.RegisterMessageHandlers(this, assemblies, typeSelector, configurationFactory))
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

        #region [====== Repositories ======]

        /// <summary>        
        /// Registers all repository types that are found in the assemblies located in the current directory and
        /// filtered by the specified <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">A search pattern that is used to locate a set of assemblies.</param>
        /// <param name="searchOption">
        /// Indicates where the assemblies should be located.
        /// </param>
        /// <param name="postfix">The postfix that every repository interface ends with. Default is <c>Repository</c>.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>        
        /// <exception cref="ArgumentException">
        /// <paramref name="searchOption"/> is not a valid option.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterRepositories(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly, string postfix = null, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            RegisterRepositories(new [] { searchPattern }, searchOption, postfix, configurationFactory);
        }

        /// <summary>        
        /// Registers all repository types that are found in the assemblies located in the current directory and
        /// filtered by the specified search patterns.
        /// </summary>
        /// <param name="searchPatternA">First search pattern that is used to locate a set of assemblies.</param>
        /// <param name="searchPatternB">Second search pattern that is used to locate a set of assemblies.</param>
        /// <param name="searchOption">
        /// Indicates where the assemblies should be located.
        /// </param>
        /// <param name="postfix">The postfix that every repository interface ends with. Default is <c>Repository</c>.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>        
        /// <exception cref="ArgumentException">
        /// <paramref name="searchOption"/> is not a valid option.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterRepositories(string searchPatternA, string searchPatternB, SearchOption searchOption = SearchOption.TopDirectoryOnly, string postfix = null, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            RegisterRepositories(new [] { searchPatternA, searchPatternB }, searchOption, postfix, configurationFactory);
        }

        /// <summary>        
        /// Registers all repository types that are found in the assemblies located in the current directory and
        /// filtered by the specified <paramref name="searchPatterns"/>.
        /// </summary>
        /// <param name="searchPatterns">A collection of search patterns that are used to locate multiple sets of assemblies.</param>
        /// <param name="searchOption">
        /// Indicates where the assemblies should be located.
        /// </param>
        /// <param name="postfix">The postfix that every repository interface ends with. Default is <c>Repository</c>.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPatterns"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="searchOption"/> is not a valid option.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterRepositories(IEnumerable<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly, string postfix = null, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            RegisterRepositories(AssemblySet.FromCurrentDirectory(searchPatterns, searchOption), postfix, configurationFactory);
        }

        /// <summary>
        /// Registers all repository interface types that are found inside the specified <paramref name="assemblies"/> with their implementations.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search through.</param>
        /// <param name="postfix">The postfix that every repository interface ends with. Default is <c>Repository</c>.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterRepositories(AssemblySet assemblies, string postfix = null, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            RegisterDependencies(assemblies, null, type => IsRepositoryInterface(type, postfix), configurationFactory);
        }

        private static bool IsRepositoryInterface(Type type, string postfix)
        {
            return type.Name.EndsWith(postfix ?? "Repository");
        }

        #endregion

        #region [====== Dependencies ======]            

        /// <summary>
        /// Registers a set of types that are found in the specified assembies
        /// and are selected by the specified <paramref name="concreteTypePredicate"/>.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search through.</param>
        /// <param name="concreteTypePredicate">A predicate that identifies which types should be registered as dependencies.</param>    
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> or <paramref name="concreteTypePredicate"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterDependencies(AssemblySet assemblies, Predicate<Type> concreteTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            DependencyClass.RegisterDependencies(this, assemblies, concreteTypePredicate, configurationFactory);
        }        

        /// <summary>
        /// Registers a set of abstract types that are mapped to a set of concrete types. Both sets are found in the specified assembies
        /// and are selected by the specified <paramref name="concreteTypePredicate"/> and <paramref name="abstractTypePredicate"/>.
        /// </summary>
        /// <param name="assemblies">A set of assemblies to search through.</param>
        /// <param name="concreteTypePredicate">A predicate that identifies which concrete types should be registered as dependencies (optional).</param>
        /// <param name="abstractTypePredicate">A predicate that identifies which abstract types should be registered as dependencies.</param>
        /// <param name="configurationFactory">
        /// Optional delegate that can be used to configure a dependency based on its type.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblies"/> or <paramref name="abstractTypePredicate"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// One or more types specified by <paramref name="abstractTypePredicate"/> match with multiple concrete implementations.
        /// </exception>        
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void RegisterDependencies(AssemblySet assemblies, Predicate<Type> concreteTypePredicate, Predicate<Type> abstractTypePredicate, Func<Type, IDependencyConfiguration> configurationFactory = null)
        {
            DependencyClass.RegisterDependencies(this, assemblies, concreteTypePredicate, abstractTypePredicate, configurationFactory);
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
