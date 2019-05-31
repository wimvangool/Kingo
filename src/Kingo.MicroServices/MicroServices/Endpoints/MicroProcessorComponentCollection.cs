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

        #region [====== AddAssemblies ======]

        /// <summary>
        /// Adds all types defined in the assemblies that match the specified search criteria to the type-set
        /// that is used to automatically register specific components.
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
        public void AddAssemblies(string searchPattern, string path = null, SearchOption searchOption = SearchOption.TopDirectoryOnly) =>
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

        private void Add(MessageHandlerClass messageHandlerClass)
        {
            switch (messageHandlerClass.Configuration.Lifetime)
            {
                case ServiceLifetime.Transient:
                    AddTransient(messageHandlerClass.Type);
                    break;
                case ServiceLifetime.Scoped:
                    AddScoped(messageHandlerClass.Type);
                    break;
                case ServiceLifetime.Singleton:
                    AddSingleton(messageHandlerClass.Type);
                    break;
                default:
                    throw NewInvalidLifetimeSpecifiedException(messageHandlerClass.Configuration.Lifetime);
            }
            _messageHandlerClasses[messageHandlerClass.Type] = messageHandlerClass;
        }

        private void AddTransient(Type type) =>
            _services.AddTransient(type);

        private void AddScoped(Type type) =>
            _services.AddScoped(type);

        private void AddSingleton(Type type) =>
            _services.AddSingleton(type);        

        private static Exception NewInvalidLifetimeSpecifiedException(ServiceLifetime lifeTime)
        {
            var messageFormat = ExceptionMessages.MessageHandlerFactoryBuilder_InvalidInstanceLifetime;
            var message = string.Format(messageFormat, lifeTime);
            return new ArgumentOutOfRangeException(message);
        }

        #endregion        

        #region [====== AddComponents ======]

        /// <summary>
        /// Adds 
        /// </summary>
        /// <param name="serviceFactory"></param>
        /// <param name="predicate"></param>
        public void AddComponents(Func<IEnumerable<Type>, IServiceCollection, IServiceCollection> serviceFactory, Func<Type, bool> predicate = null)
        {
            // TODO
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }
            throw new NotImplementedException();
        }

        #endregion

        private IEnumerable<Type> ScanTypes(Func<Type, bool> predicate = null) =>
            from type in _types
            where (predicate == null || predicate.Invoke(type)) && !IsAlreadyRegistered(type)
            select type;

        private bool IsAlreadyRegistered(Type type) =>
            _services.Any(service => service.ServiceType == type);
    }
}
