using System;

namespace ServiceComponents.ComponentModel.Server
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
        /// <param name="handler">
        /// Callback that will handle any events of type <paramtyperef name="TMessage"/>.
        /// </param>
        /// <param name="openConnection">
        /// Indicates whether or not the returned <see cref="IConnection" /> must be immediately opened.
        /// </param>
        /// <returns>The created connection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        IConnection Connect<TMessage>(Action<TMessage> handler, bool openConnection) where TMessage : class;

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
    }
}
