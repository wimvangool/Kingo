using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kingo.MicroServices.Controllers
{
    /// <summary>
    /// Represents the collection of components that a <see cref="MicroProcessor" /> uses to handle
    /// messages, execute queries and publish events.
    /// </summary>
    public sealed class MicroProcessorComponentCollection : IServiceCollectionBuilder
    {
        private readonly HashSet<MicroProcessorComponent> _searchSet;
        private readonly HashSet<MessageHandler> _messageHandlerInstances;
        private readonly IServiceCollection _messageHandlerInstanceCollection;

        private readonly HashSet<MessageHandler> _messageHandlerTypes;
        private readonly Stack<StoreAndForwardQueueType> _storeAndForwardQueueTypes;
        private readonly List<MicroProcessorComponent> _otherComponents;
        
        internal MicroProcessorComponentCollection()
        {
            _searchSet = new HashSet<MicroProcessorComponent>();
            _messageHandlerInstances = new HashSet<MessageHandler>();
            _messageHandlerInstanceCollection = new ServiceCollection();

            _messageHandlerTypes = new HashSet<MessageHandler>();
            _storeAndForwardQueueTypes = new Stack<StoreAndForwardQueueType>();
            _otherComponents = new List<MicroProcessorComponent>();
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{_searchSet.Count} type(s) in search set";       

        IServiceCollection IServiceCollectionBuilder.BuildServiceCollection(IServiceCollection services) =>
            BuildServiceCollection(services ?? new ServiceCollection());

        private IServiceCollection BuildServiceCollection(IServiceCollection services)
        {
            services = AddMessageHandlerInstancesTo(services);
            services = AddMessageHandlerTypesTo(services);
            services = AddComponentsTo(services);
            return services;
        }

        #region [====== AddMessageHandlerInstancesTo ======]

        private IServiceCollection AddMessageHandlerInstancesTo(IServiceCollection services)
        {
            foreach (var service in _messageHandlerInstanceCollection)
            {
                services.Add(service);
            }
            return services;
        }

        #endregion

        #region [====== AddMessageHandlerTypesTo ======]

        private IServiceCollection AddMessageHandlerTypesTo(IServiceCollection services) =>
            services.AddSingleton(BuildMethodFactory());

        // When building the factory, we filter out all types that have also been registered as an instance
        // to prevent weird and unpredictable behavior - instances simply take precedence in that case.
        private IHandleAsyncMethodFactory BuildMethodFactory() =>
            new HandleAsyncMethodFactory(_messageHandlerInstances.Concat(MessageHandlerTypes));

        private IEnumerable<MessageHandler> MessageHandlerTypes =>
            _messageHandlerTypes.Where(IsNotRegisteredAsInstance);

        private bool IsNotRegisteredAsInstance(MessageHandler messageHandler) =>
            _messageHandlerInstances.All(instance => instance.Type != messageHandler.Type);

        #endregion

        #region [====== AddComponentsTo ======]

        private IServiceCollection AddComponentsTo(IServiceCollection services)
        {
            // When adding all (custom) components to the service collection, we first extract all types
            // that have been added as an implementation of the IMicroServiceBus-interface. We then strip that
            // mapping from the type, so that we can replace that mapping with a single MicroServiceBus-type
            // that will delegate all work to the actual MicroServiceBus-implementations.
            var componentsToAdd = new List<MicroProcessorComponent>();
            var serviceBusTypes = new List<Type>();

            foreach (var component in MergeComponents(MessageHandlerTypes, _otherComponents))
            {
                if (component.TryRemoveServiceType(typeof(IMicroServiceBus), out var componentWithoutServiceType))
                {
                    componentsToAdd.Add(componentWithoutServiceType);
                    serviceBusTypes.Add(componentWithoutServiceType.Type);
                }
                else
                {
                    componentsToAdd.Add(component);
                }
            }
            // Secondly, we'll place the added StoreAndForwardQueue-types on top of the MicroServiceBus-implementation.
            // We do this in reverse order compared to the order in which they were added to the collection, so that
            // the following configuration...
            // 
            // Components.AddStoreAndForwardQueue<QueueOne>();
            // Components.AddStoreAndForwardQueue<QueueTwo>();
            // 
            // ...will translate the following pipeline:
            //
            // QueueOne --> QueueTwo --> MicroServiceBus.
            //
            // Note that finally, the IMicroServiceBus-mapping will be configured such that it will resolve to the
            // first item of the pipeline, which will be the first queue or just the MicroServiceBus-type if no queues
            // were added.
            IMicroServiceBusResolver microServiceBusResolver = new MicroServiceBusResolver(serviceBusTypes);

            foreach (var queueType in StoreAndForwardQueues())
            {
                componentsToAdd.Add(queueType.AssignQueueTypeFactory(microServiceBusResolver = new StoreAndForwardQueueResolver(microServiceBusResolver, queueType.Type)));
            }
            return services
                .AddComponents(componentsToAdd)
                .AddSingleton(microServiceBusResolver.ResolveMicroServiceBus);
        }

        private IEnumerable<StoreAndForwardQueueType> StoreAndForwardQueues()
        {
            while (_storeAndForwardQueueTypes.Count > 0)
            {
                yield return _storeAndForwardQueueTypes.Pop();
            }
        }

        // When adding/registering all components to a service collection, we first need to merge all the collections,
        // such that we return only a single collection where each component-type occurs only once.
        private static IEnumerable<MicroProcessorComponent> MergeComponents(params IEnumerable<MicroProcessorComponent>[] componentsToMerge)
        {
            var mergedComponents = new Dictionary<Type, MicroProcessorComponent>();

            foreach (var componentToAdd in componentsToMerge.SelectMany(components => components))
            {
                if (mergedComponents.TryGetValue(componentToAdd.Type, out var component))
                {
                    mergedComponents[componentToAdd.Type] = component.MergeWith(componentToAdd);
                }
                else
                {
                    mergedComponents.Add(componentToAdd.Type, componentToAdd);
                }
            }
            return mergedComponents.Values;
        }

        #endregion

        #region [====== AddToSearchSet ======]

        /// <summary>
        /// Adds a type to the searchable type-set.
        /// </summary>
        /// <typeparam name="TComponent">Type to add.</typeparam>
        public void AddToSearchSet<TComponent>() =>
            AddToSearchSet(typeof(TComponent));

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
        public void AddToSearchSet(string searchPattern, string path = null, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
            AddToSearchSet(TypeSet.Empty.Add(searchPattern, path, searchOption));

        /// <summary>
        /// Adds the specified <paramref name="types"/> to the searchable type-set.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddToSearchSet(params Type[] types) =>
            AddToSearchSet(types as IEnumerable<Type>);

        /// <summary>
        /// Adds the specified <paramref name="types"/> to the searchable type-set.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddToSearchSet(IEnumerable<Type> types) =>
            _searchSet.UnionWith(MicroProcessorComponent.FromTypes(types));

        #endregion

        #region [====== AddMessageHandlers (Types) ======]

        /// <summary>
        /// Automatically registers all message handlers that are found in the types that were added to this collection,
        /// which are types that implement the <see cref="IMessageHandler{TMessage}"/> interface.
        /// </summary>
        /// <returns>The number of message handlers that were added.</returns> 
        public int AddMessageHandlers()
        {
            var messageHandlerTypes = MessageHandlerType.FromComponents(_searchSet).ToArray();

            foreach (var messageHandler in messageHandlerTypes)
            {
                AddMessageHandler(messageHandler);
            }
            return messageHandlerTypes.Length;
        }

        /// <summary>
        /// Adds the specified <typeparamref name="TMessageHandler"/> as a message handler for the processor, if and only if
        /// this type represents a message handler.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the specified <typeparamref name="TMessageHandler"/> was added as a message handler; otherwise <c>false</c>.
        /// </returns>
        /// <typeparam name="TMessageHandler">Type of the message handler to add.</typeparam>
        public bool AddMessageHandler<TMessageHandler>() where TMessageHandler : class =>
            AddMessageHandler(typeof(TMessageHandler));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a message handler for the processor, if and only if
        /// this type represents a message handler.
        /// </summary>
        /// <param name="type">The type to add as a message handler.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="type"/> was added as a message handler; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool AddMessageHandler(Type type)
        {
            if (MessageHandlerType.IsMessageHandlerComponent(type, out var messageHandler))
            {
                AddMessageHandler(messageHandler);
                return true;
            }
            return false;
        }

        private void AddMessageHandler(MessageHandler messageHandler) =>
            _messageHandlerTypes.Add(messageHandler);

        #endregion

        #region [====== AddMessageHandler (Instances) ======]

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
            if (MessageHandlerInstance.IsMessageHandlerInstance(messageHandler, out var instance) && _messageHandlerInstances.Add(instance))
            {
                _messageHandlerInstanceCollection.AddComponent(instance, messageHandler);                
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
        public void AddMessageHandler<TMessage>(Action<TMessage, IMessageHandlerOperationContext> messageHandler) =>
            AddMessageHandler(new MessageHandlerInstance<TMessage>(messageHandler));        

        /// <summary>
        /// Adds the specified <paramref name="messageHandler" /> as a singleton instance.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="messageHandler"/>.</typeparam>
        /// <param name="messageHandler">The handler to register.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Func<TMessage, IMessageHandlerOperationContext, Task> messageHandler) =>
            AddMessageHandler(new MessageHandlerInstance<TMessage>(messageHandler));        

        private void AddMessageHandler<TMessage>(MessageHandlerInstance<TMessage> messageHandler)
        {
            if (_messageHandlerInstances.Add(messageHandler))
            {
                _messageHandlerInstanceCollection.AddTransient<IMessageHandler<TMessage>>(provider => messageHandler);
            }            
        }

        #endregion

        #region [====== AddQueries ======]

        /// <summary>
        /// Automatically registers all queries that are found in the types that were added to this collection, which
        /// are types that implement the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/>
        /// interface.
        /// </summary>        
        public void AddQueries() =>
            AddComponents(QueryType.FromComponent);

        /// <summary>
        /// Adds <typeparamref name="TQuery"/> as a query, if and only if it is a valid component
        /// that implements the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/> interface.
        /// </summary>
        /// <typeparam name="TQuery">Type of the query to add.</typeparam>
        /// <returns>
        /// <c>true</c> if the specified <typeparamref name="TQuery"/> was added as a query; otherwise <c>false</c>.
        /// </returns>
        public bool AddQuery<TQuery>() where TQuery : class =>
            AddQuery(typeof(TQuery));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a query, if and only if the specified type is a valid component
        /// that implements the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/> interface.
        /// </summary>
        /// <param name="type">The type to add as a query.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="type"/> was added as a query; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool AddQuery(Type type) =>
            AddComponent(type, QueryType.FromComponent);

        #endregion

        #region [====== AddStoreAndForwardQueue ======]

        /// <summary>
        /// Adds <typeparamref name="TQueue"/> as a <see cref="StoreAndForwardQueue"/> that will be placed on top
        /// of the <see cref="IMicroServiceBus"/> pipeline.
        /// </summary>
        /// <typeparam name="TQueue">The type to register as a controller.</typeparam>
        /// <returns>
        /// <c>true</c> if <typeparamref name="TQueue"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        public bool AddStoreAndForwardQueue<TQueue>() where TQueue : StoreAndForwardQueue =>
            AddStoreAndForwardQueue(typeof(TQueue));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="StoreAndForwardQueue"/> that will be placed on top
        /// of the <see cref="IMicroServiceBus"/> pipeline, if and only if <paramref name="type"/> is a <see cref="StoreAndForwardQueue"/>.
        /// </summary>
        /// <param name="type">The type to register as a queue.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> was added as a queue; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool AddStoreAndForwardQueue(Type type)
        {
            if (StoreAndForwardQueueType.IsStoreAndForwardQueue(type, out var queueType))
            {
                _storeAndForwardQueueTypes.Push(queueType);
                return true;
            }
            return false;
        }

        #endregion

        #region [====== AddMicroServiceBusController ======]

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="IHostedService" />.
        /// If <paramref name="isMainController"/> is <c>true</c>, the controller is also registered as
        /// a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <typeparamref name="TController"/> will be registered
        /// as the <see cref="IMicroServiceBus"/> of this service.
        /// </param>
        /// <returns>
        /// <c>true</c> if <typeparamref name="TController"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        public bool AddMicroServiceBusController<TController>(bool isMainController = false) where TController : MicroServiceBusController =>
            AddMicroServiceBusController(typeof(TController), isMainController);

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="IHostedService" /> if and only if
        /// <paramref name="type"/> is a <see cref="MicroServiceBusController" />.
        /// If <paramref name="isMainController"/> is <c>true</c>, the controller is also registered as
        /// a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <param name="type">The type to register as a controller.</param>
        /// <param name="isMainController">
        /// Indicates whether or not the specified controller is owned by the current service.
        /// If <c>true</c>, the specified controller <paramref name="type"/> will be registered as the <see cref="IMicroServiceBus"/>
        /// of this service.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="type"/> was added as a controller; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool AddMicroServiceBusController(Type type, bool isMainController = false) =>
            AddComponent(type, component => MicroServiceBusControllerType.FromComponent(component, isMainController));

        #endregion

        #region [====== AddMicroServiceBus ======]

        /// <summary>
        /// Adds <typeparamref name="TServiceBus"/> as a service bus if it is a valid component.
        /// </summary>
        /// <typeparam name="TServiceBus">Type of the service bus to add.</typeparam>
        /// <returns>
        /// <c>true</c> if the specified <typeparamref name="TServiceBus"/> was added as a query; otherwise <c>false</c>.
        /// </returns>
        public bool AddMicroServiceBus<TServiceBus>() where TServiceBus : class, IMicroServiceBus =>
            AddMicroServiceBus(typeof(TServiceBus));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a service bus, if and only if the specified type is a valid component
        /// that implements the <see cref="IMicroServiceBus"/> interface.
        /// </summary>
        /// <param name="type">The type to add as a service bus.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="type"/> was added as a service bus; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public bool AddMicroServiceBus(Type type) =>
            AddComponent(type, MicroServiceBusType.FromComponent);

        #endregion

        #region [====== AddComponents ======]

        /// <summary>
        /// Adds a number of components by looking through the search set and selecting/adding a
        /// sub-set of these components to this collection.
        /// </summary>
        /// <param name="componentFactory">
        /// Delegate that is used to filter and/or create the component to add. If the delegate returns <c>null</c>,
        /// that component is ignored (filtered out). Otherwise, the returned component (which can be a different
        /// component from the one that is passed into the delegate) is added to this collection.
        /// </param>
        /// <returns>The number of components that were added.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="componentFactory"/> is <c>null</c>.
        /// </exception>
        public int AddComponents(Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory)
        {
            var componentsToAdd = ComponentsToAdd(componentFactory).ToArray();

            foreach (var componentToAdd in componentsToAdd)
            {
                AddComponent(componentToAdd);
            }
            return componentsToAdd.Length;
        }

        private IEnumerable<MicroProcessorComponent> ComponentsToAdd(Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory)
        {
            if (componentFactory == null)
            {
                throw new ArgumentNullException(nameof(componentFactory));
            }
            foreach (var component in _searchSet)
            {
                if (TryCreateComponent(componentFactory, component, out var componentToAdd))
                {
                    yield return componentToAdd;
                }
            }
        }        

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a component if and only if the specified <paramref name="componentFactory"/>
        /// returns a component to add. Note that <paramref name="componentFactory"/> is only invoked if the specified
        /// <paramref name="type"/> is a valid component.
        /// </summary>
        /// <param name="type">Type to add as a component.</param>
        /// <param name="componentFactory">
        /// Delegate that is used to filter and/or create the component to add. If the delegate returns <c>null</c>,
        /// that component is ignored (filtered out). Otherwise, the returned component (which can be a different
        /// component from the one that is passed into the delegate) is added to this collection.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="type"/> was added as a component; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="componentFactory"/> is <c>null</c>.
        /// </exception>
        public bool AddComponent(Type type, Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory)
        {
            if (componentFactory == null)
            {
                throw new ArgumentNullException(nameof(componentFactory));
            }
            if (MicroProcessorComponent.IsMicroProcessorComponent(type, out var component) && TryCreateComponent(componentFactory, component, out var componentToAdd))
            {
                AddComponent(componentToAdd);
                return true;
            }
            return false;
        }

        private void AddComponent(MicroProcessorComponent component) =>
            _otherComponents.Add(component);

        private static bool TryCreateComponent(Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory, MicroProcessorComponent component, out MicroProcessorComponent componentToAdd) =>
            (componentToAdd = componentFactory.Invoke(component)) != null;

        #endregion
    }
}
