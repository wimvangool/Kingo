using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace YellowFlare.MessageProcessing
{
    /// <summary>
    /// When implemented, represents a factory of all message-handlers that are used to handle the messages
    /// for the <see cref="MessageProcessor"/>.
    /// </summary>
    public abstract class MessageHandlerFactory
    {
        private readonly List<MessageHandlerClass> _messageHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory" /> class.
        /// </summary>
        protected MessageHandlerFactory()
        {                        
            _messageHandlers = new List<MessageHandlerClass>();
        }

        /// <summary>
        /// Registers all message-handler types that are found in the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to scan for message-handlers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerFactory RegisterMessageHandlers(Assembly assembly)
        {
            return RegisterMessageHandlers(assembly, null);
        }

        /// <summary>
        /// Registers all message-handler types that are found in the specified <paramref name="assembly"/> and
        /// satisfy the specified <paramref name="predicate"/>.
        /// </summary>
        /// <param name="assembly">The assembly to scan for message-handlers.</param>
        /// <param name="predicate">Optional predicate to filter specified message-handler types.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerFactory RegisterMessageHandlers(Assembly assembly, Func<Type, bool> predicate)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            foreach (var type in assembly.GetTypes())
            {
                MessageHandlerClass handler;

                if (MessageHandlerClass.TryRegisterIn(this, type, predicate, out handler))
                {
                    _messageHandlers.Add(handler);
                }
            }
            return this;
        }

        /// <summary>
        /// Registers all message-handler types that are found in the assemblies deployed to the current directory.
        /// </summary>
        public MessageHandlerFactory RegisterMessageHandlersInCurrentDirectory()
        {
            return RegisterMessageHandlersInCurrentDirectory(null);
        }

        /// <summary>
        /// Registers all message-handler types that are found in the assemblies deployed to the current directory
        /// and satisfy the specified <paramref name="predicate"/>.
        /// </summary>
        public MessageHandlerFactory RegisterMessageHandlersInCurrentDirectory(Func<Type, bool> predicate)
        {
            foreach (var assembly in FindAssembliesInCurrentDirectory())
            {
                RegisterMessageHandlers(assembly, predicate);
            }
            return this;
        }        

        /// <summary>
        /// Registers a certain type to resolve with the default lifetime.
        /// </summary>
        /// <param name="type">A concrete type.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        protected internal abstract void RegisterWithPerResolveLifetime(Type type);

        /// <summary>
        /// Registers a certain type to resolve with a lifetime that spans a single command execution.
        /// </summary>
        /// <param name="type">A concrete type.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        protected internal abstract void RegisterWithPerUnitOfWorkLifetime(Type type);

        /// <summary>
        /// Registers a certain type to resolve with a lifetime equivalent to that of the container iself.
        /// </summary>
        /// <param name="type">A concrete type.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>        
        protected internal abstract void RegisterSingle(Type type);        

        /// <inheritdoc />
        internal IEnumerable<IMessageHandler<TMessage>> CreateInternalHandlersFor<TMessage>(TMessage message) where TMessage : class
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var matchedHandlers =
                from handlerClass in _messageHandlers
                let handlers = handlerClass.ResolveInEveryRoleForInternal(message.GetType())
                from handler in handlers
                select handler;
            
            return matchedHandlers.Cast<IMessageHandler<TMessage>>();
        }

        /// <inheritdoc />
        internal IEnumerable<IMessageHandler<TMessage>> CreateExternalHandlersFor<TMessage>(TMessage message) where TMessage : class
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            var matchedHandlers =
                from handlerClass in _messageHandlers
                let handlers = handlerClass.ResolveInEveryRoleForExternal(message.GetType())
                from handler in handlers
                select handler;

            return matchedHandlers.Cast<IMessageHandler<TMessage>>();
        }

        /// <summary>
        /// Resolves an instance of the requested type.
        /// </summary>
        /// <param name="type">Type to resolve.</param>
        /// <returns>An instance of the requested type.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>                
        protected internal abstract object Resolve(Type type);

        private static IEnumerable<Assembly> FindAssembliesInCurrentDirectory()
        {
            return from assemblyFileName in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dll", SearchOption.TopDirectoryOnly)
                   select Assembly.LoadFile(assemblyFileName);
        }
    }
}
