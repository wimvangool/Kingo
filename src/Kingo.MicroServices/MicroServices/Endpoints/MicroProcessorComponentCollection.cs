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
        private readonly HashSet<MessageHandler> _messageHandlerInstances;
        private readonly HashSet<MessageHandler> _messageHandlerTypes;
        private readonly HashSet<MicroServiceBusControllerType> _serviceBusControllerTypes;
        private readonly HashSet<MicroServiceBusType> _serviceBusTypes;
        private readonly HashSet<MicroProcessorComponent> _components;
        private readonly IServiceCollection _services;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentCollection" /> class.
        /// </summary>
        public MicroProcessorComponentCollection()
        {            
            _messageHandlerInstances = new HashSet<MessageHandler>();
            _messageHandlerTypes = new HashSet<MessageHandler>();
            _serviceBusControllerTypes = new HashSet<MicroServiceBusControllerType>();
            _serviceBusTypes = new HashSet<MicroServiceBusType>();
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
            return services
                .AddSingleton(BuildMethodFactory())
                .AddTransient(BuildMicroServiceBus);
        }

        // When building the factory, we filter out all types that have also been registered as an instance
        // to prevent weird and unpredictable behavior - instances simply take precedence in that case.
        private IHandleAsyncMethodFactory BuildMethodFactory() =>
            new HandleAsyncMethodFactory(_messageHandlerInstances.Concat(_messageHandlerTypes.Where(IsNotRegisteredAsInstance)));

        private bool IsNotRegisteredAsInstance(MessageHandler messageHandler) =>
            _messageHandlerInstances.All(instance => instance.Type != messageHandler.Type);
        
        // When building the service bus, we actually create a composite bus that encapsulates all registered service bus types.
        // Only this composite bus is actually registered as an IMicroServiceBus instance, so that clients always get the right
        // service bus instance.        
        internal IMicroServiceBus BuildMicroServiceBus(IServiceProvider provider) =>
            new MicroServiceBusComposite(_serviceBusTypes.Select(bus => provider.GetRequiredService(bus.Type)).OfType<IMicroServiceBus>());

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

        #region [====== AddMessageHandlers (Types) ======]

        /// <summary>
        /// Automatically registers all message handlers that are found in the types that were added to this collection,
        /// which are types that implement the <see cref="IMessageHandler{TMessage}"/> interface.
        /// </summary>        
        /// <param name="predicate">Optional predicate that is used to filter specific types.</param>                
        public void AddMessageHandlers(Func<MicroProcessorComponent, bool> predicate = null) =>
            AddMessageHandlers(GetComponents(predicate));        
        
        private void AddMessageHandlers(IEnumerable<MicroProcessorComponent> components)
        {
            foreach (var messageHandler in MessageHandlerType.FromComponents(components))
            {
                AddMessageHandler(messageHandler);
            }
        }

        /// <summary>
        /// Adds the specified <typeparamref name="TMessageHandler"/> as a message handler for the processor, if and only if
        /// this type represents a message handler.
        /// </summary>
        /// <typeparam name="TMessageHandler">Type of the message handler to add.</typeparam>
        public void AddMessageHandler<TMessageHandler>() =>
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
        
        private void AddMessageHandler(MessageHandler messageHandler)
        {
            if (_messageHandlerTypes.Add(messageHandler))
            {
                _services.AddComponent(messageHandler, messageHandler.Interfaces.Select(@interface => @interface.Type));
            }            
        }

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
            if (_messageHandlerInstances.Add(messageHandler))
            {
                _services.AddTransient<IMessageHandler<TMessage>>(provider => messageHandler);
            }            
        }

        #endregion

        #region [====== AddQueries ======]

        /// <summary>
        /// Automatically registers all queries that are found in the types that were added to this collection, which
        /// are types that implement the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/>
        /// interface.
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

        #region [====== AddMicroServiceBusController ======]

        /// <summary>
        /// Automatically registers all types that are a <see cref="MicroServiceBusController" />. Each controller
        /// will also be registered as a <see cref="Microsoft.Extensions.Hosting.IHostedService"/> that will be
        /// started and stopped automatically.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter specific types to scan.</param>
        public void AddMicroServiceBusControllers(Func<MicroProcessorComponent, bool> predicate = null)
        {
            foreach (var controller in MicroServiceBusControllerType.FromComponents(GetComponents(predicate)))
            {
                AddMicroServiceBusController(controller);
            }
        }

        /// <summary>
        /// Adds <typeparamref name="TController"/> as a <see cref="MicroServiceBusController"/>. If
        /// <typeparamref name="TController"/> implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>,
        /// it is also registered as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <typeparam name="TController">The type to register as a controller.</typeparam> 
        public void AddMicroServiceBusController<TController>() where TController : MicroServiceBusController =>
            AddMicroServiceBusController(typeof(TController));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="MicroServiceBusController"/> if and only if
        /// the specified <paramref name="type"/> actually is a <see cref="MicroServiceBusController"/>. If <paramref name="type"/>
        /// implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>, it is also registered
        /// as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <param name="type">The type to register as a controller.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public void AddMicroServiceBusController(Type type)
        {
            if (MicroServiceBusControllerType.IsMicroProcessorBusControllerType(type, out var controller))
            {
                AddMicroServiceBusController(controller);
            }
        }

        private void AddMicroServiceBusController(MicroServiceBusControllerType controller)
        {
            if (_serviceBusControllerTypes.Add(controller))
            {
                _services.AddComponent(controller, controller.ServiceTypes);
            }
        }

        #endregion

        #region [====== AddMicroServiceBus ======]

        /// <summary>
        /// Automatically registers all types that implement the <see cref="IMicroServiceBus" /> interface as
        /// a service bus endpoint. Any type that also implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>
        /// is also registered as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter specific types to scan.</param>
        public void AddMicroServiceBuses(Func<MicroProcessorComponent, bool> predicate = null)
        {
            foreach (var microServiceBus in MicroServiceBusType.FromComponents(GetComponents(predicate)))
            {
                AddMicroServiceBus(microServiceBus);
            }
        }

        /// <summary>
        /// Adds the specified <typeparamref name="TMicroServiceBus"/> as a <see cref="IMicroServiceBus" />.
        /// If <typeparamref name="TMicroServiceBus"/> implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>,
        /// it is also registered as a hosted service that will be started and stopped automatically.
        /// </summary>
        /// <typeparam name="TMicroServiceBus">Type of a service bus.</typeparam>
        public void AddMicroServiceBus<TMicroServiceBus>() where TMicroServiceBus : IMicroServiceBus =>
            AddMicroServiceBus(typeof(TMicroServiceBus));

        /// <summary>
        /// Adds the specified <paramref name="type"/> as a <see cref="IMicroServiceBus" /> instance, if
        /// and only if this type implements the <see cref="IMicroServiceBus"/> interface. If <paramref name="type"/>
        /// also implements <see cref="Microsoft.Extensions.Hosting.IHostedService"/>, it is also registered
        /// as a hosted service that will be started and stopped automatically.
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

        private void AddMicroServiceBus(MicroServiceBusType microServiceBus)
        {
            if (_serviceBusTypes.Add(microServiceBus))
            {
                _services.AddComponent(microServiceBus, microServiceBus.ServiceTypes);
            }            
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
