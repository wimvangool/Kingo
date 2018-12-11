using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents a builder of <see cref="IMessageHandlerFactory"/> instances.
    /// </summary>
    public abstract class MessageHandlerFactoryBuilder
    {
        private IEnumerable<MessageHandlerClass> _messageHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactoryBuilder" /> class.
        /// </summary>
        protected MessageHandlerFactoryBuilder()
        {
            _messageHandlers = Enumerable.Empty<MessageHandlerClass>();
        }

        #region [====== Type Registration ======]

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
        public void Register(string searchPattern, Func<Type, bool> predicate) =>
            Register(searchPattern, null, predicate);

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
        public void Register(string searchPattern, string path = null, Func<Type, bool> predicate = null) =>
            RegisterMessageHandlers(ScanTypes(searchPattern, path, predicate));

        private static IEnumerable<Type> ScanTypes(string searchPattern, string path = null, Func<Type, bool> predicate = null) =>
            from type in TypeSet.Empty.Add(searchPattern, path)
            where predicate == null || predicate.Invoke(type)
            select type;

        /// <summary>
        /// Registers all types of the specified <paramref name="types"/> that implement
        /// <see cref="IMessageHandler{T}" /> as message handlers. The exact behavior and lifetime
        /// of these handlers will be determined by the values or their <see cref="MessageHandlerAttribute" />.
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public void RegisterMessageHandlers(IEnumerable<Type> types) =>
            _messageHandlers = _messageHandlers.Concat(RegisterMessageHandlerClasses(types ?? throw new ArgumentNullException(nameof(types))));

        private IEnumerable<MessageHandlerClass> RegisterMessageHandlerClasses(IEnumerable<Type> types)
        {
            foreach (var messageHandlerClass in MessageHandlerClass.FromTypes(types))
            {
                yield return Register(messageHandlerClass);
            }
        }

        private MessageHandlerClass Register(MessageHandlerClass messageHandlerClass)
        {
            switch (messageHandlerClass.Configuration.Lifetime)
            {
                case ServiceLifetime.Transient:
                    RegisterTransient(messageHandlerClass.Type);
                    break;
                case ServiceLifetime.Scoped:
                    RegisterScoped(messageHandlerClass.Type);
                    break;
                case ServiceLifetime.Singleton:
                    RegisterSingleton(messageHandlerClass.Type);
                    break;
                default:
                    throw NewInvalidLifetimeSpecifiedException(messageHandlerClass.Configuration.Lifetime);
            }
            return messageHandlerClass;
        }        

        private static Exception NewInvalidLifetimeSpecifiedException(ServiceLifetime lifeTime)
        {
            var messageFormat = ExceptionMessages.MessageHandlerFactoryBuilder_InvalidInstanceLifetime;
            var message = string.Format(messageFormat, lifeTime);
            return new ArgumentOutOfRangeException(message);
        }

        #endregion

        #region [====== Type Registration (Per Specific Lifetime) ======]                       

        /// <summary>
        /// Registers the specified <paramref name="type" /> with a transient lifetime.
        /// </summary>
        /// <param name="type">The type to register.</param>        
        protected abstract void RegisterTransient(Type type);

        /// <summary>
        /// Registers the specified <paramref name="type" /> with a scoped lifetime.
        /// </summary>
        /// <param name="type">The type to register.</param>      
        protected abstract void RegisterScoped(Type type);

        /// <summary>
        /// Registers the specified <paramref name="type" /> with as a singleton.
        /// </summary>
        /// <param name="type">The type to register.</param>  
        protected abstract void RegisterSingleton(Type type);

        #endregion        

        #region [====== Build ======]

        /// <summary>
        /// Builds and returns a new <see cref="IMessageHandlerFactory" />.
        /// </summary>
        /// <returns>A new <see cref="IMessageHandlerFactory" />.</returns>
        public virtual IMessageHandlerFactory Build() =>
            new MessageHandlerFactory(_messageHandlers, CreateServiceProvider());

        /// <summary>
        /// Creates and returns a new <see cref="IServiceProvider" /> that can resolve all message handlers
        /// that have been registered with this builder.
        /// </summary>
        /// <returns>A new service provider.</returns>
        protected abstract IServiceProvider CreateServiceProvider();

        #endregion
    }
}
