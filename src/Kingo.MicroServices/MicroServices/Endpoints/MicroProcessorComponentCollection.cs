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
        private readonly List<MessageHandler> _messageHandlerInstances;
        private readonly List<MessageHandler> _messageHandlerTypes;
        private readonly HashSet<MicroProcessorComponent> _components;
        private readonly IServiceCollection _services;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentCollection" /> class.
        /// </summary>
        public MicroProcessorComponentCollection()
        {            
            _messageHandlerInstances = new List<MessageHandler>();
            _messageHandlerTypes = new List<MessageHandler>();
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

        internal IHandleAsyncMethodFactory BuildMethodFactory()
        {
            // When building the factory, we select only distinct instances and types, so no instance or type is ever
            // invoked twice for the same message. We also filter out all types that have also been registered as an instance
            // to prevent weird and unpredictable behavior - instances simply take precedence in that case.
            var instances = _messageHandlerInstances.Distinct().ToArray();
            var types = _messageHandlerTypes.Distinct().Where(messageHandler => instances.All(instance => instance.Type != messageHandler.Type));
            return new HandleAsyncMethodFactory(instances.Concat(types));
        }                    

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
            if (MessageHandlerInstance.IsMessageHandlerInstance(messageHandler, out var instance))
            {
                _messageHandlerInstances.Add(instance);

                foreach (var @interface in instance.Interfaces)
                {                    
                    _services.AddTransient(@interface.Type, provider => provider.GetService(instance.Type));
                }
                _services.AddTransient(messageHandler.GetType(), provider => messageHandler);
            }            
        }        

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance. NB: this message handler will only
        /// receive internal messages (events).
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Action<TMessage, MessageHandlerOperationContext> messageHandler) =>
            AddMessageHandler(new MessageHandlerInstance<TMessage>(messageHandler));        

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Func<TMessage, MessageHandlerOperationContext, Task> messageHandler) =>
            AddMessageHandler(new MessageHandlerInstance<TMessage>(messageHandler));        

        private void AddMessageHandler<TMessage>(MessageHandlerInstance<TMessage> messageHandler)
        {
            _messageHandlerInstances.Add(messageHandler);
            _services.AddTransient<IMessageHandler<TMessage>>(provider => messageHandler);            
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
                _messageHandlerTypes.Add(messageHandler);
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
