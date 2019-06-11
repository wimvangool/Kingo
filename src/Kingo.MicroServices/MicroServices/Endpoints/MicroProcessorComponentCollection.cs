using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// Represents the collection of components that a <see cref="MicroProcessor" /> uses to handle
    /// messages, execute queries and publish events.
    /// </summary>
    public sealed class MicroProcessorComponentCollection : IServiceCollectionBuilder
    {        
        private readonly Dictionary<Type, MessageHandlerType> _messageHandlerComponents;
        private readonly List<MessageHandlerInstance> _messageHandlerInstances;
        private readonly HashSet<MicroProcessorComponent> _components;
        private readonly IServiceCollection _services;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentCollection" /> class.
        /// </summary>
        public MicroProcessorComponentCollection()
        {
            _messageHandlerComponents = new Dictionary<Type, MessageHandlerType>();
            _messageHandlerInstances = new List<MessageHandlerInstance>();
            _components = new HashSet<MicroProcessorComponent>();
            _services = new ServiceCollection();            
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{_components.Count} type(s) added, {_services.Count} type registration(s) made.";

        IServiceCollection IServiceCollectionBuilder.BuildServiceCollection(IServiceCollection services) =>
            BuildServiceCollection(services ?? new ServiceCollection());

        private IServiceCollection BuildServiceCollection(IServiceCollection services)
        {
            foreach (var service in _services)
            {
                services.Add(service);
            }
            return services.AddSingleton(BuildMethodFactory());                
        }

        internal IHandleAsyncMethodFactory BuildMethodFactory() =>
            new HandleAsyncMethodFactory(_messageHandlerComponents.Values.Cast<IHandleAsyncMethodFactory>().Concat(_messageHandlerInstances));

        #region [====== AddTypes ======]

        /// <summary>
        /// Adds a type to the searchable type-set.
        /// </summary>
        /// <typeparam name="TComponent">Type to add.</typeparam>
        public void AddType<TComponent>() =>
            AddTypes(typeof(TComponent));

        /// <summary>
        /// Adds all types defined in the assemblies that match the specified search criteria to the
        /// searchable type-set.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="path">A path pointing to a specific directory. If <c>null</c>, the <see cref="TypeSet.CurrentDirectory"/> is used.</param>
        /// <param name="searchOption">
        /// Indicates whether or not only the top-level directory is to be searched.        
        /// </param>
        /// <returns>A new set containing all types from the specified assemblies.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission to access the specified path or its files.
        /// </exception>
        public void AddTypes(string searchPattern, string path = null, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            AddTypes(TypeSet.Empty.Add(searchPattern, path, searchOption));

        /// <summary>
        /// Adds the specified <paramref name="types"/> to the searchable type-set.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddTypes(params Type[] types) =>
            AddTypes(types as IEnumerable<Type>);

        /// <summary>
        /// Adds the specified <paramref name="types"/> to the searchable type-set.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddTypes(IEnumerable<Type> types) =>
            _components.UnionWith(MicroProcessorComponent.FromTypes(types));
                    
        #endregion        

        #region [====== AddMessageHandlers ======]

        /// <summary>
        /// Adds the specified <paramref name="messageHandler"/> as a singleton instance for every <see cref="IMessageHandler{TMessage}"/>
        /// implementation it has. If <paramref name="messageHandler"/> does not implement this interface, it is simply ignored.
        /// </summary>
        /// <param name="messageHandler">The handler to register.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler(object messageHandler)
        {
            var instances = MessageHandlerInstance.FromInstance(messageHandler).ToArray();
            if (instances.Length == 0)
            {
                return;
            }
            foreach (var instance in instances)
            {
                _messageHandlerInstances.Add(instance);
                _services.AddTransient(instance.Interface.Type, provider => provider.GetService(instance.Type));
            }
            _services.AddTransient(messageHandler.GetType(), provider => messageHandler);
        }

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <param name="handlesExternalMessages">
        /// Indicates whether or not the specified <paramref name="messageHandler"/> will handle external messages from the processor.
        /// </param>
        /// <param name="handlesInternalMessages">
        /// Indicates whether or not the specified <paramref name="messageHandler"/> will handle internal messages from the processor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Action<TMessage, MessageHandlerOperationContext> messageHandler, bool handlesExternalMessages, bool handlesInternalMessages) =>
            AddMessageHandler(messageHandler, MessageHandlerAttribute.Create(handlesExternalMessages, handlesInternalMessages));

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <param name="configuration">Optional explicit configuration for the specified <paramref name="messageHandler"/>.</param> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Action<TMessage, MessageHandlerOperationContext> messageHandler, IMessageHandlerConfiguration configuration = null) =>
            AddMessageHandler(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configuration);

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <param name="handlesExternalMessages">
        /// Indicates whether or not the specified <paramref name="messageHandler"/> will handle external messages from the processor.
        /// </param>
        /// <param name="handlesInternalMessages">
        /// Indicates whether or not the specified <paramref name="messageHandler"/> will handle internal messages from the processor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, bool handlesExternalMessages, bool handlesInternalMessages) =>
            AddMessageHandler(messageHandler, MessageHandlerAttribute.Create(handlesExternalMessages, handlesInternalMessages));

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>
        /// <param name="configuration">Optional explicit configuration for the specified <paramref name="messageHandler"/>.</param> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler, IMessageHandlerConfiguration configuration = null) =>
            AddMessageHandler(MessageHandlerDecorator<TMessage>.Decorate(messageHandler), configuration);

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(IMessageHandler<TMessage> messageHandler) =>
            AddMessageHandler(messageHandler, null);

        private void AddMessageHandler<TMessage>(IMessageHandler<TMessage> messageHandler, IMessageHandlerConfiguration configuration)
        {
            _messageHandlerInstances.Add(new MessageHandlerInstance<TMessage>(messageHandler, configuration));
            _services.AddTransient(provider => messageHandler);
            _services.AddTransient(messageHandler.GetType(), provider => messageHandler);
        }

        /// <summary>
        /// Adds all message handlers that are found in the assemblies that match the specified search criteria.
        /// </summary>        
        /// <param name="predicate">Optional predicate that is used to filter specific types.</param>                
        public void AddMessageHandlers(Func<MicroProcessorComponent, bool> predicate = null) =>
            AddMessageHandlers(GetComponents(predicate));        
        
        private void AddMessageHandlers(IEnumerable<MicroProcessorComponent> components)
        {
            foreach (var messageHandler in MessageHandlerType.FromComponents(components))
            {
                _services.AddComponent(messageHandler, messageHandler.Interfaces.Select(@interface => @interface.Type));
                _messageHandlerComponents[messageHandler.Type] = messageHandler;
            }
        }       

        #endregion

        #region [====== AddQueries ======]

        /// <summary>
        /// Adds all queries to the components of the processor. A query is defined as any public, non-abstract, non-generic class that
        /// implements at least one variation of the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/>
        /// interfaces.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter specific types to scan.</param>
        public void AddQueries(Func<MicroProcessorComponent, bool> predicate = null) =>
            AddComponents(AddQueries, predicate);

        private static IServiceCollection AddQueries(IEnumerable<MicroProcessorComponent> components, IServiceCollection services)
        {
            foreach (var query in QueryType.FromComponents(components))
            {
                services = services.AddComponent(query, query.Interfaces.Select(@interface => @interface.Type));
            }            
            return services;
        }        

        #endregion

        #region [====== AddComponents ======]

        /// <summary>
        /// Adds a number of components by scanning and automatically registering a number of types.
        /// </summary>
        /// <param name="serviceFactory">Delegate used to scan and register the components into a <see cref="IServiceCollection"/>.</param>
        /// <param name="predicate">Optional predicate to filter specific types to scan.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serviceFactory"/> is <c>null</c>.
        /// </exception>
        public void AddComponents(Func<IEnumerable<MicroProcessorComponent>, IServiceCollection, IServiceCollection> serviceFactory, Func<MicroProcessorComponent, bool> predicate = null)
        {            
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }
            foreach (var service in serviceFactory.Invoke(GetComponents(predicate), new ServiceCollection()))
            {                
                _services.Add(service);
            }
        }

        private IEnumerable<MicroProcessorComponent> GetComponents(Func<MicroProcessorComponent, bool> predicate = null) =>
            from component in _components
            where predicate == null || predicate.Invoke(component)
            select component;

        #endregion
    }
}
