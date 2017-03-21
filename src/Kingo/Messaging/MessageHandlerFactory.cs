using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented, represents a factory of all message-handlers that are used to handle the messages
    /// for the <see cref="MicroProcessor"/>.
    /// </summary>
    public abstract class MessageHandlerFactory : IReadOnlyCollection<MessageHandlerClass>
    {
        [DebuggerDisplay("Count = {_messageHandlers.Count}")]
        private readonly List<MessageHandlerClass> _messageHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerFactory" /> class.
        /// </summary>        
        protected MessageHandlerFactory()
        {
            _messageHandlers = new List<MessageHandlerClass>();
        }

        /// <inheritdoc />
        public int Count =>
            _messageHandlers.Count;

        /// <inheritdoc />
        public IEnumerator<MessageHandlerClass> GetEnumerator() =>
            _messageHandlers.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() =>
            _messageHandlers.GetEnumerator();

        /// <inheritdoc />
        public override string ToString() =>
            $"{_messageHandlers.Count} MessageHandler(s) Registered";

        #region [====== MessageHandlers ======]        
             
        internal void RegisterMessageHandlers(IEnumerable<Type> types)
        {
            foreach (var messageHandler in MessageHandlerClass.RegisterMessageHandlers(this, types))
            {
                _messageHandlers.Add(messageHandler);
            }
        }

        /// <inheritdoc />
        internal IEnumerable<MessageHandler<TMessage>> ResolveMessageHandlers<TMessage>(MessageHandlerContext context, TMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return from handlerClass in _messageHandlers
                   let handlers = handlerClass.CreateInstancesInEveryRoleFor(this, context, message)
                   from handler in handlers
                   select handler;
        }

        /// <summary>
        /// Create an instance of the requested message handler type.
        /// </summary>
        /// <param name="type">Type to create.</param>
        /// <returns>An instance of the requested message handler type.</returns>                       
        protected internal abstract object Resolve(Type type);

        #endregion

        #region [====== Type Registration (Per Resolve Lifetime) ======]

        /// <summary>
        /// Registers the specified type <typeparamref name="T" />. A new instance of this type will be created each time it is
        /// resolved.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        public MessageHandlerFactory RegisterWithPerResolveLifetime<T>() =>
            RegisterWithPerResolveLifetime(typeof(T));

        /// <summary>
        /// Registers the specified <paramref name="type" />. A new instance of this type will be created each time it is
        /// resolved.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerFactory RegisterWithPerResolveLifetime(Type type) =>
            RegisterWithPerResolveLifetime(null, type);

        /// <summary>
        /// Registers a the specified type <typeparamref name="TTo" /> as an implementation of type <typeparamref name="TFrom" />.
        /// A new instance of this type will be created each time an instance of type <typeparamref name="TFrom"/> is requested.
        /// </summary>
        /// <typeparam name="TFrom">A base type of <typeparamref name="TTo"/> or interface implemented by <typeparamref name="TTo"/>.</typeparam>
        /// <typeparam name="TTo">The type to register.</typeparam>
        public MessageHandlerFactory RegisterWithPerResolveLifetime<TFrom, TTo>() where TTo : TFrom =>
            RegisterWithPerResolveLifetime(typeof(TFrom), typeof(TTo));

        /// <summary>
        /// Registers the specified type <paramref name="to" />. A new instance of this type will be created each time it is
        /// resolved. If <paramref name="from"/> is specified, a new instance of <paramref name="to"/> will be created
        /// when an instance of <paramref name="from"/> is requested.
        /// </summary>
        /// <param name="from">A base type of <paramref name="to"/> or interface implemented by <paramref name="to"/> (optional).</param>
        /// <param name="to">The type to register.</param>        
        /// <returns>This instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="to"/> is <c>null</c>.
        /// </exception>
        public abstract MessageHandlerFactory RegisterWithPerResolveLifetime(Type @from, Type to);

        #endregion

        #region [====== Type Registration (Per UnitOfWork Lifetime) ======]

        /// <summary>
        /// Registers the specified type <typeparamref name="T" />. Only one instance of this type will be created inside the scope of
        /// a unit of work, represented by the current <see cref="MicroProcessorContext" />.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        public MessageHandlerFactory RegisterWithPerUnitOfWorkLifetime<T>() =>
            RegisterWithPerUnitOfWorkLifetime(typeof(T));

        /// <summary>
        /// Registers the specified <paramref name="type" />. A new instance of this type will be created each time it is
        /// resolved.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerFactory RegisterWithPerUnitOfWorkLifetime(Type type) =>
            RegisterWithPerUnitOfWorkLifetime(null, type);

        /// <summary>
        /// Registers a the specified type <typeparamref name="TTo" /> as an implementation of type <typeparamref name="TFrom" />.
        /// An instance of <typeparamref name="TTo"/> will be resolved when an instance of <typeparamref name="TFrom"/> is requested and
        /// only one instance of this type will be created inside the scope of a unit of work, represented by the current
        /// <see cref="MicroProcessorContext" />.       
        /// </summary>
        /// <typeparam name="TFrom">A base type of <typeparamref name="TTo"/> or interface implemented by <typeparamref name="TTo"/>.</typeparam>
        /// <typeparam name="TTo">The type to register.</typeparam>
        public MessageHandlerFactory RegisterWithPerUnitOfWorkLifetime<TFrom, TTo>() where TTo : TFrom =>
            RegisterWithPerUnitOfWorkLifetime(typeof(TFrom), typeof(TTo));

        /// <summary>
        /// Registers the specified type <paramref name="to" />. Only one instance of this type will be created inside the scope of
        /// a unit of work, represented by the current <see cref="MicroProcessorContext" />. If <paramref name="from"/> is specified,
        /// an instance of <paramref name="to"/> will be resolved when an instance of <paramref name="from"/> is requested.
        /// </summary>
        /// <param name="from">A base type of <paramref name="to"/> or interface implemented by <paramref name="to"/> (optional).</param>
        /// <param name="to">The type to register.</param>        
        /// <returns>This instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="to"/> is <c>null</c>.
        /// </exception>     
        public abstract MessageHandlerFactory RegisterWithPerUnitOfWorkLifetime(Type @from, Type to);

        #endregion

        #region [====== Type Registration (Singletons) ======]

        /// <summary>
        /// Registers the specified type <typeparamref name="T" />. Only one instance of this type will ever be created by this factory,
        /// reflecting the singleton pattern.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        public MessageHandlerFactory RegisterSingleton<T>()
            => RegisterSingleton(typeof(T));

        /// <summary>
        /// Registers the specified <paramref name="type" />. Only one instance of this type will ever be created by this factory,
        /// reflecting the singleton pattern.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public MessageHandlerFactory RegisterSingleton(Type type)
            => RegisterSingleton(null, type);

        /// <summary>
        /// Registers a the specified type <typeparamref name="TTo" /> as an implementation of type <typeparamref name="TFrom" />.
        /// An instance of <typeparamref name="TTo"/> will be resolved when an instance of <typeparamref name="TFrom"/> is requested and
        /// only one instance of this type will be created by this factory, reflecting the singleton pattern.       
        /// </summary>
        /// <typeparam name="TFrom">A base type of <typeparamref name="TTo"/> or interface implemented by <typeparamref name="TTo"/>.</typeparam>
        /// <typeparam name="TTo">The type to register.</typeparam>
        public MessageHandlerFactory RegisterSingleton<TFrom, TTo>() where TTo : TFrom =>
            RegisterSingleton(typeof(TFrom), typeof(TTo));

        /// <summary>
        /// Registers the specified type <paramref name="to" />. Only one instance of this type will ever be created by this factory,
        /// reflecting the singleton pattern. If <paramref name="from"/> is specified, an instance of <paramref name="to"/> will be resolved
        /// when an instance of <paramref name="from"/> is requested.
        /// </summary>
        /// <param name="from">A base type of <paramref name="to"/> or interface implemented by <paramref name="to"/> (optional).</param>
        /// <param name="to">The type to register.</param>        
        /// <returns>This instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="to"/> is <c>null</c>.
        /// </exception> 
        public abstract MessageHandlerFactory RegisterSingleton(Type @from, Type to);

        /// <summary>
        /// Registers a certain instance as a singleton.
        /// </summary>        
        /// <param name="instance">The instance to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <c>null</c>.
        /// </exception> 
        public MessageHandlerFactory RegisterSingleton(object instance) =>
            RegisterSingleton(null, instance);

        /// <summary>
        /// Registers a certain instance as a singleton. The specified <paramref name="instance"/>
        /// will be returned when an instance of <typeparamref name="TFrom"/> is requested.
        /// </summary>
        /// <typeparam name="TFrom">A base type of <typeparamref name="TFrom"/> or interface implemented by <paramref name="instance"/>.</typeparam>        
        /// <param name="instance">The instance to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <c>null</c>.
        /// </exception> 
        public MessageHandlerFactory RegisterSingleton<TFrom>(TFrom instance) =>
            RegisterSingleton(typeof(TFrom), instance);

        /// <summary>
        /// Registers a certain instance as a singleton. If <paramref name="from"/> is specified,
        /// <paramref name="to"/> will be returned when an instance of <paramref name="from"/> is requested.
        /// </summary>
        /// <param name="from">A base type of <paramref name="to"/> or interface implemented by <paramref name="to"/> (optional).</param>
        /// <param name="to">The instance to register.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="to"/> is <c>null</c>.
        /// </exception> 
        public abstract MessageHandlerFactory RegisterSingleton(Type @from, object to);

        #endregion
    }
}
