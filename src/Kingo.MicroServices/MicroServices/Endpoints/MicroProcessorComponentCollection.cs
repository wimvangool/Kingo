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
        private readonly Dictionary<Type, MessageHandlerClass> _messageHandlerClasses;
        private readonly List<MessageHandlerInstance> _messageHandlerInstances;        
        private readonly IServiceCollection _services;
        private TypeSet _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicroProcessorComponentCollection" /> class.
        /// </summary>
        public MicroProcessorComponentCollection()
        {
            _messageHandlerClasses = new Dictionary<Type, MessageHandlerClass>();
            _messageHandlerInstances = new List<MessageHandlerInstance>();            
            _services = new ServiceCollection();
            _types = TypeSet.Empty;            
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Registered handlers: {_messageHandlerClasses.Count} class(es), {_messageHandlerInstances.Count} instances";

        IServiceCollection IServiceCollectionBuilder.BuildServiceCollection(IServiceCollection services) =>
            BuildServiceCollection(services ?? new ServiceCollection());

        private IServiceCollection BuildServiceCollection(IServiceCollection services)
        {
            foreach (var service in _services)
            {
                services.Add(service);
            }
            return services.AddSingleton(BuildMessageHandlerFactory());                
        }

        internal IMessageHandlerFactory BuildMessageHandlerFactory() =>
            new MessageHandlerFactory(_messageHandlerClasses.Values.Cast<IMessageHandlerFactory>().Concat(_messageHandlerInstances));

        #region [====== AddTypes ======]

        /// <summary>
        /// Adds the specified <paramref name="types"/> to the searchable type-set.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddTypes(params Type[] types) =>
            _types = _types.Add(types);

        /// <summary>
        /// Adds the specified <paramref name="types"/> to the searchable type-set.
        /// </summary>
        /// <param name="types">The types to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddTypes(IEnumerable<Type> types) =>
            _types = _types.Add(types);

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
            _types = _types.Add(searchPattern, path, searchOption);

        #endregion        

        #region [====== AddMessageHandlers ======]

        /// <summary>
        /// Adds the specified <paramref name="handler"/> as a singleton instance for every <see cref="IMessageHandler{TMessage}"/>
        /// implementation is has. If <paramref name="handler"/> does not implement this interface, it is simply ignored.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        /// <param name="operationTypes">
        /// Optional set of operation types for which the handler will be invoked. If <c>null</c>, the operation types defined
        /// by the <see cref="MessageHandlerAttribute" />, if declared on <paramref name="handler"/>, will be used. If not found,
        /// the handler will be invoked by all operation types.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler(object handler, MicroProcessorOperationTypes? operationTypes = null)
        {
            foreach (var instance in MessageHandlerInstance.FromHandler(handler, operationTypes))
            {
                _messageHandlerInstances.Add(instance);
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="handler" /> for the (optionally specified) <paramref name="operationTypes" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="handler"/>.</typeparam>
        /// <param name="handler">The handler to register.</param>
        /// <param name="operationTypes">
        /// Optional set of operation types for which the handler will be invoked. If <c>null</c>,
        /// the handler will be invoked by all operation types.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Action<TMessage, MessageHandlerContext> handler, MicroProcessorOperationTypes? operationTypes = null) =>
            AddMessageHandler(MessageHandlerDecorator<TMessage>.Decorate(handler), operationTypes);

        /// <summary>
        /// Adds the specified <paramref name="handler" /> for the (optionally specified) <paramref name="operationTypes" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="handler"/>.</typeparam>
        /// <param name="handler">The handler to register.</param>
        /// <param name="operationTypes">
        /// Optional set of operation types for which the handler will be invoked. If <c>null</c>,
        /// the handler will be invoked by all operation types.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(Func<TMessage, MessageHandlerContext, Task> handler, MicroProcessorOperationTypes? operationTypes = null) =>
            AddMessageHandler(MessageHandlerDecorator<TMessage>.Decorate(handler), operationTypes);

        /// <summary>
        /// Adds the specified <paramref name="handler" /> for the (optionally specified) <paramref name="operationTypes" />.
        /// </summary>
        /// <typeparam name="TMessage">Type of the message that is handled by the specified <paramref name="handler"/>.</typeparam>
        /// <param name="handler">The handler to register.</param>
        /// <param name="operationTypes">
        /// Optional set of operation types for which the handler will be invoked. If <c>null</c>, the operation types defined
        /// by the <see cref="MessageHandlerAttribute" />, if declared on <paramref name="handler"/>, will be used. If not found,
        /// the handler will be invoked by all operation types.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandler<TMessage>(IMessageHandler<TMessage> handler, MicroProcessorOperationTypes? operationTypes = null) =>
            _messageHandlerInstances.Add(new MessageHandlerInstance<TMessage>(handler, operationTypes));

        /// <summary>
        /// Adds the specified <typeparamref name="TMessageHandler"/>, if it's a valid message handler type.
        /// </summary>
        /// <typeparam name="TMessageHandler">Type of message handler to register.</typeparam>
        public void AddMessageHandler<TMessageHandler>() =>
            AddMessageHandlers(typeof(TMessageHandler));

        /// <summary>
        /// Adds all message handlers that are found in the assemblies that match the specified search criteria.
        /// </summary>        
        /// <param name="predicate">Optional predicate that is used to filter specific types from the assemblies.</param>
        /// <returns>The factory that contains all registered message handlers.</returns>               
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void AddMessageHandlers(Func<Type, bool> predicate = null) =>
            AddMessageHandlers(ScanTypes(predicate));

        /// <summary>
        /// Adds the specified message handler <paramref name="types"/>, for each type that is a valid message handler type.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandlers(params Type[] types) =>
            AddMessageHandlers(types as IEnumerable<Type>);

        /// <summary>
        /// Adds all types of the specified <paramref name="types"/> that implement
        /// <see cref="IMessageHandler{T}" /> as message handlers. The exact behavior and lifetime
        /// of these handlers will be determined by the values or their <see cref="MessageHandlerAttribute" />.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void AddMessageHandlers(IEnumerable<Type> types)
        {
            foreach (var messageHandlerClass in MessageHandlerClass.FromTypes(types))
            {
                Add(messageHandlerClass);
            }
        }                   

        private void Add(MessageHandlerClass messageHandler)
        {
            switch (messageHandler.Configuration.Lifetime)
            {
                case ServiceLifetime.Transient:
                    AddTransient(messageHandler);
                    break;
                case ServiceLifetime.Scoped:
                    AddScoped(messageHandler);
                    break;
                case ServiceLifetime.Singleton:
                    AddSingleton(messageHandler);
                    break;
                default:
                    throw NewInvalidLifetimeSpecifiedException(messageHandler.Configuration.Lifetime);
            }
            _messageHandlerClasses[messageHandler.Type] = messageHandler;
        }

        private void AddTransient(MessageHandlerClass messageHandler) =>
            _services.AddTransient(messageHandler.Type).AddTransient(messageHandler.RegistrationType);

        private void AddScoped(MessageHandlerClass messageHandler) =>
            _services.AddTransient(messageHandler.Type).AddScoped(messageHandler.RegistrationType);

        private void AddSingleton(MessageHandlerClass messageHandler) =>
            _services.AddTransient(messageHandler.Type).AddSingleton(messageHandler.RegistrationType);        

        private static Exception NewInvalidLifetimeSpecifiedException(ServiceLifetime lifeTime)
        {
            var messageFormat = ExceptionMessages.MessageHandlerFactoryBuilder_InvalidInstanceLifetime;
            var message = string.Format(messageFormat, lifeTime);
            return new ArgumentOutOfRangeException(message);
        }

        #endregion

        #region [====== AddQueries ======]

        /// <summary>
        /// Adds all queries to the components of the processor. A query is defined as any public, non-abstract, non-generic class that
        /// implements at least one variation of the <see cref="IQuery{TResponse}"/> or <see cref="IQuery{TRequest, TResponse}"/>
        /// interfaces.
        /// </summary>
        /// <param name="predicate">Optional predicate to filter specific types to scan.</param>
        public void AddQueries(Func<Type, bool> predicate = null) =>
            AddComponents(AddQueries, predicate);

        private static IServiceCollection AddQueries(IEnumerable<Type> types, IServiceCollection services)
        {
            foreach (var type in types)
            {
                var interfaceTypes = type.GetInterfaces(typeof(IQuery<>), typeof(IQuery<,>)).ToArray();
                if (interfaceTypes.Length == 0)
                {
                    continue;
                }
                foreach (var interfaceType in interfaceTypes)
                {
                    services = services.AddTransient(interfaceType, provider => provider.GetRequiredService(type));
                }
                services = services.AddTransient(type);
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
        public void AddComponents(Func<IEnumerable<Type>, IServiceCollection, IServiceCollection> serviceFactory, Func<Type, bool> predicate = null)
        {            
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }
            foreach (var service in serviceFactory.Invoke(ScanTypes(predicate), new ServiceCollection()))
            {
                _services.Add(service);
            }
        }

        #endregion

        private IEnumerable<Type> ScanTypes(Func<Type, bool> predicate = null) =>
            from type in _types
            where CanBeAddedAsComponent(type)
            where predicate == null || predicate.Invoke(type)
            where !IsAlreadyRegistered(type, _services)
            select type;

        private static bool CanBeAddedAsComponent(Type type) =>
            type.IsClass && !type.IsAbstract && (type.IsPublic || type.IsNestedPublic) && !type.ContainsGenericParameters;

        private static bool IsAlreadyRegistered(Type type, IServiceCollection services) =>
            services.Any(service => service.ImplementationType == type);
    }
}
