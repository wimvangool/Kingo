namespace System.ComponentModel.Server
{
    /// <summary>
    /// Server as a bas class for all <see cref="MessageHandlerModule">MessageHandlerModules</see> that associate
    /// each message with a specific <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <typeparam name="TStrategy">Type of the strategy to associate with each message.</typeparam>
    public abstract class MessageHandlerModule<TStrategy> : MessageHandlerModule where TStrategy : class
    {                
        private static readonly MessageToStrategyMapping<TStrategy> _EmptyMapping = new MessageToStrategyMapping<TStrategy>();

        /// <summary>
        /// Returns the mapping of certain messages to specific strategies.
        /// </summary>
        public IMessageToStrategyMapping<TStrategy> StrategyMapping
        {
            get { return MessageToStrategyMapping.GetOrAdd(GetType(), CreateMessageToStrategyMapping); }
        }

        /// <summary>
        /// Creates and returns a <see cref="IMessageToStrategyMapping{T}" /> that contains a strategy
        /// for certain messages or message types.
        /// </summary>
        /// <returns>A <see cref="IMessageToStrategyMapping{T}" />.</returns>
        protected virtual IMessageToStrategyMapping<TStrategy> CreateMessageToStrategyMapping()
        {
            return _EmptyMapping;
        }

        /// <summary>
        /// The default strategy to use when the message being handled is not explicitly associated with a specific strategy.
        /// </summary>
        protected abstract TStrategy DefaultStrategy
        {
            get;
        }       

        /// <summary>
        /// Attempts to retrieve the associated strategy for the message on the specified <paramref name="handler"/>
        /// and then invokes <see cref="Invoke(IMessageHandler, TStrategy)" />.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        public override void Invoke(IMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            TStrategy strategy;

            if (StrategyMapping.TryGetStrategy(handler.Message, out strategy))
            {
                Invoke(handler, strategy);
                return;
            }
            if (Message.TryGetStrategyFromAttribute(handler.Message, out strategy))
            {
                Invoke(handler, strategy);
                return;
            }
            Invoke(handler, DefaultStrategy);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> using the specified <paramref name="strategy"/>.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <param name="strategy">The strategy to use.</param>
        protected abstract void Invoke(IMessageHandler handler, TStrategy strategy);
    }
}
