namespace System.ComponentModel.Server
{
    /// <summary>
    /// When implemented by a class, represents a <see cref="MessageProcessorBus" /> to which event-handlers can subscribe.
    /// </summary>
    public interface IMessageProcessorBus : IDomainEventBus
    {
        #region [====== Connect ======]

        /// <summary>
        /// Connects the specified handler to the bus.
        /// </summary>
        /// <param name="handler">The handler to connect.</param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>        
        IConnection Connect(object handler, bool openConnection);

        /// <summary>
        /// Connects the specified callback to the bus.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="action">
        /// Callback that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        IConnection Connect<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class;

        /// <summary>
        /// Connects the specified handler to the bus.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="handler">
        /// Handler that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        IConnection Connect<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class;

        #endregion

        #region [====== ConnectThreadLocal ======]

        /// <summary>
        /// Connects the specified handler to the bus, which will only receive those events that are
        /// published on the current thread.
        /// </summary>
        /// <param name="handler">The handler to connect.</param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>        
        IConnection ConnectThreadLocal(object handler, bool openConnection);

        /// <summary>
        /// Connects the specified callback to the bus, which will only receive those events that are
        /// published on the current thread.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="action">
        /// Callback that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is <c>null</c>.
        /// </exception>
        IConnection ConnectThreadLocal<TMessage>(Action<TMessage> action, bool openConnection) where TMessage : class;

        /// <summary>
        /// Connects the specified handler to the bus, which will only receive those events that are
        /// published on the current thread.
        /// </summary>
        /// <typeparam name="TMessage">Type of event to listen to.</typeparam>
        /// <param name="handler">
        /// Handler that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        IConnection ConnectThreadLocal<TMessage>(IMessageHandler<TMessage> handler, bool openConnection) where TMessage : class;

        #endregion
    }
}
