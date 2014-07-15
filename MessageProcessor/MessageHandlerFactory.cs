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
        /// Registers all message-handler types that are found in the assemblies deployed to the directory
        /// of the calling assembly.
        /// </summary>        
        public void RegisterMessageHandlers()
        {
            RegisterMessageHandlers(Assembly.GetCallingAssembly().Location, null);
        }

        /// <summary>
        /// Registers all message-handler types that are found in the assemblies deployed to the directory
        /// of the calling assembly and also satisfy the specified predicate.
        /// </summary>
        /// <param name="predicate">A predicate that filters the requires message-handlers to register (optional).</param>        
        public void RegisterMessageHandlers(Func<Type, bool> predicate)
        {
            RegisterMessageHandlers(Assembly.GetCallingAssembly().Location, predicate);
        }   
     
        /// <summary>
        /// Registers all message-handler types that are found in the assemblies deployed to the specified
        /// directory.
        /// </summary>
        /// <param name="directory">The directory to scan.</param>        
        public void RegisterMessageHandlers(string directory)
        {
            RegisterMessageHandlers(directory, null);
        }

        /// <summary>
        /// Registers all message-handler types that are found in the assemblies deployed to the specified
        /// directory and also satisfy the specified predicate.
        /// </summary>
        /// <param name="directory">The directory to scan.</param>
        /// <param name="predicate">A predicate that filters the requires message-handlers to register (optional).</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="directory"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="directory"/> is not a valid directory.
        /// </exception>
        public void RegisterMessageHandlers(string directory, Func<Type, bool> predicate)
        {            
            foreach (var assembly in FindAssembliesIn(directory))
            {
                RegisterMessageHandlersFrom(assembly, predicate);
            }           
        }

        /// <summary>
        /// Registers all message-handler types that are found in the assembly with the specified <paramref name="assemblyName"/>.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to scan for message-handlers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assemblyName"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="assemblyName"/> is not a valid path to an existing assembly.
        /// </exception>
        public void RegisterMessageHandlersFrom(string assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }
            if (Path.IsPathRooted(assemblyName))
            {
                RegisterMessageHandlersFrom(Assembly.LoadFrom(assemblyName));
                return;
            }
            string relativeRoot = Assembly.GetCallingAssembly().Location;
            string assemblyLocation = Path.Combine(relativeRoot, assemblyName);

            RegisterMessageHandlersFrom(Assembly.LoadFrom(assemblyLocation));
        }

        /// <summary>
        /// Registers all message-handler types that are found in the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to scan for message-handlers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="assembly"/> is <c>null</c>.
        /// </exception>
        public void RegisterMessageHandlersFrom(Assembly assembly)
        {
            RegisterMessageHandlersFrom(assembly, null);
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
        public void RegisterMessageHandlersFrom(Assembly assembly, Func<Type, bool> predicate)
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
        internal IEnumerable<IMessageHandlerPipeline<TMessage>> CreateMessageHandlersFor<TMessage>(TMessage message) where TMessage : class
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            return from handlerClass in _messageHandlers
                   let handlers = handlerClass.CreateInstancesInEveryRoleFor(message)
                   from handler in handlers
                   select handler;
        }        

        /// <summary>
        /// Create an instance of the requested message handler type.
        /// </summary>
        /// <param name="type">Type to create.</param>
        /// <returns>An instance of the requested message handler type.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>                
        protected internal abstract object CreateMessageHandler(Type type);

        internal static IMessageHandlerPipeline<TMessage> CreateMessageHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
        {
            return MessageHandlerClass.CreateMessageHandler(handler);
        }

        internal static IMessageHandlerPipeline<TMessage> CreateMessageHandler<TMessage>(Action<TMessage> action) where TMessage : class
        {
            return MessageHandlerClass.CreateMessageHandler(action);
        }

        private static IEnumerable<Assembly> FindAssembliesIn(string directory)
        {
            return from assemblyFileName in Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly)
                   select Assembly.LoadFile(assemblyFileName);
        }
    }
}
