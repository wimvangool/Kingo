using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

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
        private readonly HashSet<MicroServiceBusType> _serviceBusTypes;
        private readonly List<MicroProcessorComponent> _otherComponents;
        
        internal MicroProcessorComponentCollection()
        {
            _searchSet = new HashSet<MicroProcessorComponent>();
            _messageHandlerInstances = new HashSet<MessageHandler>();
            _messageHandlerInstanceCollection = new ServiceCollection();

            _messageHandlerTypes = new HashSet<MessageHandler>();            
            _serviceBusTypes = new HashSet<MicroServiceBusType>();    
            _otherComponents = new List<MicroProcessorComponent>();
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{_searchSet.Count} type(s) in search set";       

        IServiceCollection IServiceCollectionBuilder.BuildServiceCollection(IServiceCollection services) =>
            BuildServiceCollection(services ?? new ServiceCollection());

        private IServiceCollection BuildServiceCollection(IServiceCollection services)
        {
            foreach (var service in _messageHandlerInstanceCollection)
            {
                services.Add(service);
            }
            return services
                .AddSingleton(BuildMethodFactory())
                .AddTransient(BuildMicroServiceBus)
                .AddComponents(MergeComponents());
        }

        // When building the factory, we filter out all types that have also been registered as an instance
        // to prevent weird and unpredictable behavior - instances simply take precedence in that case.
        private IHandleAsyncMethodFactory BuildMethodFactory() =>
            new HandleAsyncMethodFactory(_messageHandlerInstances.Concat(MessageHandlerTypes));

        private IEnumerable<MessageHandler> MessageHandlerTypes =>
            _messageHandlerTypes.Where(IsNotRegisteredAsInstance);

        private bool IsNotRegisteredAsInstance(MessageHandler messageHandler) =>
            _messageHandlerInstances.All(instance => instance.Type != messageHandler.Type);

        // When building the service bus, we actually create a composite bus that encapsulates all registered service bus types.
        // Only this composite bus is actually registered as an IMicroServiceBus instance, so that clients always get the right
        // service bus instance.        
        internal IMicroServiceBus BuildMicroServiceBus(IServiceProvider provider) =>
            new MicroServiceBusComposite(_serviceBusTypes.Select(bus => provider.GetRequiredService(bus.Type)).OfType<IMicroServiceBus>());

        // When adding/registering all components to a service collection, we first need to merge all the collections,
        // such that we return only a single collection where each component-type occurs only once.
        private IEnumerable<MicroProcessorComponent> MergeComponents()
        {
            var components = new Dictionary<Type, MicroProcessorComponent>();
            AddOrMerge(components, MessageHandlerTypes);
            AddOrMerge(components, _serviceBusTypes);
            AddOrMerge(components, _otherComponents);
            return components.Values;
        }

        private static void AddOrMerge(IDictionary<Type, MicroProcessorComponent> components, IEnumerable<MicroProcessorComponent> componentsToAdd)
        {
            foreach (var componentToAdd in componentsToAdd)
            {
                AddOrMerge(components, componentToAdd);                    
            }
        }

        private static void AddOrMerge(IDictionary<Type, MicroProcessorComponent> components, MicroProcessorComponent componentToAdd)
        {
            if (components.TryGetValue(componentToAdd.Type, out var component))
            {
                components[componentToAdd.Type] = component.MergeWith(componentToAdd);
            }
            else
            {
                components.Add(componentToAdd.Type, componentToAdd);
            }
        }

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
        public void AddMessageHandlers()
        {
            foreach (var messageHandler in MessageHandlerType.FromComponents(_searchSet))
            {
                AddMessageHandler(messageHandler);
            }
        }               

        /// <summary>
        /// Adds the specified <typeparamref name="TMessageHandler"/> as a message handler for the processor, if and only if
        /// this type represents a message handler.
        /// </summary>
        /// <typeparam name="TMessageHandler">Type of the message handler to add.</typeparam>
        public void AddMessageHandler<TMessageHandler>() where TMessageHandler : class =>
            AddMessageHandler(typeof(TMessageHandler));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a message handler for the processor, if and only if
        /// this type represents a message handler.
        /// </summary>
        /// <param name="type">The type to add as a message handler.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler(Type type)
        {
            if (MessageHandlerType.IsMessageHandlerComponent(type, out var messageHandler))
            {
                AddMessageHandler(messageHandler);
            }
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
        public void AddQuery<TQuery>() where TQuery : class =>
            AddQuery(typeof(TQuery));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a query, if and only if the specified type is a valid component
        /// that implements the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/> interface.
        /// </summary>
        /// <param name="type">The type to add as a query.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public void AddQuery(Type type) =>
            AddComponent(type, QueryType.FromComponent);

        #endregion

        #region [====== AddMicroServiceBus ======]

        /// <summary>
        /// Automatically registers all types that implement the <see cref="IMicroServiceBus" /> interface.
        /// </summary>        
        public void AddMicroServiceBuses()
        {
            foreach (var microServiceBus in MicroServiceBusType.FromComponents(_searchSet))
            {
                AddMicroServiceBus(microServiceBus);
            }
        }

        /// <summary>
        /// Adds the specified <typeparamref name="TMicroServiceBus"/> as a <see cref="IMicroServiceBus" />.
        /// </summary>
        /// <typeparam name="TMicroServiceBus">Type of a service bus.</typeparam>
        public void AddMicroServiceBus<TMicroServiceBus>() where TMicroServiceBus : class, IMicroServiceBus =>
            AddMicroServiceBus(typeof(TMicroServiceBus));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="IMicroServiceBus" /> instance, if
        /// and only if this type implements the <see cref="IMicroServiceBus"/> interface.
        /// </summary>
        /// <param name="type">The type to register as a service bus.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public void AddMicroServiceBus(Type type)
        {
            if (MicroServiceBusType.IsMicroServiceBusType(type, out var microServiceBus))
            {
                AddMicroServiceBus(microServiceBus);
            }
        }

        private void AddMicroServiceBus(MicroServiceBusType microServiceBus) =>
            _serviceBusTypes.Add(microServiceBus);

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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="componentFactory"/> is <c>null</c>.
        /// </exception>
        public void AddComponents(Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory)
        {
            foreach (var componentToAdd in ComponentsToAdd(componentFactory))
            {
                AddComponent(componentToAdd);
            }
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> or <paramref name="componentFactory"/> is <c>null</c>.
        /// </exception>
        public void AddComponent(Type type, Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory)
        {
            if (componentFactory == null)
            {
                throw new ArgumentNullException(nameof(componentFactory));
            }
            if (MicroProcessorComponent.IsMicroProcessorComponent(type, out var component) && TryCreateComponent(componentFactory, component, out var componentToAdd))
            {
                AddComponent(componentToAdd);
            }
        }

        private void AddComponent(MicroProcessorComponent component) =>
            _otherComponents.Add(component);

        private static bool TryCreateComponent(Func<MicroProcessorComponent, MicroProcessorComponent> componentFactory, MicroProcessorComponent component, out MicroProcessorComponent componentToAdd) =>
            (componentToAdd = componentFactory.Invoke(component)) != null;

        #endregion
    }
}
