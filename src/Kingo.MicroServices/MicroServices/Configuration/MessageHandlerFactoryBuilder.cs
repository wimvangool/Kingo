using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Kingo.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented, represents a builder of <see cref="IMessageHandlerFactory"/> instances.
    /// </summary>
    public sealed class MessageHandlerFactoryBuilder : IServiceCollectionConfigurator
    {        
        private readonly Dictionary<Type, MessageHandlerClass> _messageHandlerClasses;
        private readonly List<MessageHandlerInstance> _messageHandlerInstances;
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactoryBuilder" /> class.
        /// </summary>
        public MessageHandlerFactoryBuilder(IServiceCollection services = null)
        {
            _messageHandlerClasses = new Dictionary<Type, MessageHandlerClass>();
            _messageHandlerInstances = new List<MessageHandlerInstance>();
            _services = services ?? new ServiceCollection();            
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"Registered handlers: {_messageHandlerClasses.Count} class(es), {_messageHandlerInstances.Count} instances";

        void IServiceCollectionConfigurator.Configure(IServiceCollection services)
        {
            foreach (var service in _services)
            {
                services.Add(service);
            }
            services.AddSingleton(Build());
        }

        #region [====== Registration ======]

        /// <summary>
        /// Registers the specified <paramref name="handler"/> as a singleton instance for every <see cref="IMessageHandler{TMessage}"/>
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
        public void Add(object handler, MicroProcessorOperationTypes? operationTypes = null)
        {
            foreach (var instance in MessageHandlerInstance.FromHandler(handler, operationTypes))
            {
                _messageHandlerInstances.Add(instance);
            }
        }

        /// <summary>
        /// Registers the specified <paramref name="handler" /> for the (optionally specified) <paramref name="operationTypes" />.
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
        public void Add<TMessage>(Action<TMessage, MessageHandlerContext> handler, MicroProcessorOperationTypes? operationTypes = null) =>
            Add(MessageHandlerDecorator<TMessage>.Decorate(handler), operationTypes);

        /// <summary>
        /// Registers the specified <paramref name="handler" /> for the (optionally specified) <paramref name="operationTypes" />.
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
        public void Add<TMessage>(Func<TMessage, MessageHandlerContext, Task> handler, MicroProcessorOperationTypes? operationTypes = null) =>
            Add(MessageHandlerDecorator<TMessage>.Decorate(handler), operationTypes);

        /// <summary>
        /// Registers the specified <paramref name="handler" /> for the (optionally specified) <paramref name="operationTypes" />.
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
        public void Add<TMessage>(IMessageHandler<TMessage> handler, MicroProcessorOperationTypes? operationTypes = null) =>
            _messageHandlerInstances.Add(new MessageHandlerInstance<TMessage>(handler, operationTypes));

        /// <summary>
        /// Registers the specified <typeparamref name="TMessageHandler"/>, if it's a valid message handler type.
        /// </summary>
        /// <typeparam name="TMessageHandler">Type of message handler to register.</typeparam>
        public void Add<TMessageHandler>() =>
            Add(typeof(TMessageHandler));

        /// <summary>
        /// Registers all message handlers that are found in the assemblies that match the specified search criteria.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>        
        /// <param name="predicate">Optional predicate that is used to filter specific types from the assemblies.</param>
        /// <returns>The factory that contains all registered message handlers.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void Add(string searchPattern, Func<Type, bool> predicate) =>
            Add(searchPattern, null, predicate);

        /// <summary>
        /// Registers all message handlers that are found in the assemblies that match the specified search criteria.
        /// </summary>
        /// <param name="searchPattern">The pattern that is used to match specified files/assemblies.</param>
        /// <param name="path">A path pointing to a specific directory. If <c>null</c>, the <see cref="TypeSet.CurrentDirectory"/> is used.</param>
        /// <param name="predicate">Optional predicate that is used to filter specific types from the assemblies.</param>
        /// <returns>The factory that contains all registered message handlers..</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="searchPattern"/> is <c>null</c>.
        /// </exception>        
        /// <exception cref="IOException">
        /// An error occurred while reading files from the specified location(s).
        /// </exception>
        /// <exception cref="SecurityException">
        /// The caller does not have the required permission
        /// </exception>
        public void Add(string searchPattern, string path = null, Func<Type, bool> predicate = null) =>
            Add(ScanTypes(searchPattern, path, predicate));

        private static IEnumerable<Type> ScanTypes(string searchPattern, string path = null, Func<Type, bool> predicate = null) =>
            from type in TypeSet.Empty.Add(searchPattern, path)
            where predicate == null || predicate.Invoke(type)
            select type;        

        /// <summary>
        /// Registers the specified message handler <paramref name="types"/>, for each type that is a valid message handler type.
        /// </summary>
        /// <param name="types">Types to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="types"/> is <c>null</c>.
        /// </exception>
        public void Add(params Type[] types) =>
            Add(types as IEnumerable<Type>);

        /// <summary>
        /// Registers all types of the specified <paramref name="types"/> that implement
        /// <see cref="IMessageHandler{T}" /> as message handlers. The exact behavior and lifetime
        /// of these handlers will be determined by the values or their <see cref="MessageHandlerAttribute" />.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public void Add(IEnumerable<Type> types)
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

        #region [====== Build ======]        

        /// <summary>
        /// Builds and returns a new <see cref="IMessageHandlerFactory" />.
        /// </summary>
        /// <returns>A new <see cref="IMessageHandlerFactory" />.</returns>
        public IMessageHandlerFactory Build() =>
            new MessageHandlerFactory(_messageHandlerClasses.Values, _messageHandlerInstances, _services);       

        #endregion
    }
}
