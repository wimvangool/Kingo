namespace System.ComponentModel.Server
{
    /// <summary>
    /// Server as a bas class for all <see cref="MessageHandlerModule">MessageHandlerModules</see> that associate
    /// each message with a specific <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <typeparam name="TStrategy">Type of the strategy to associate with each message.</typeparam>
    public abstract class MessageHandlerModule<TStrategy> : MessageHandlerModule where TStrategy : class
    {
        private readonly MessageToStrategyMapping<TStrategy> _strategyMapping;        

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerModule" /> class.
        /// </summary>                
        protected MessageHandlerModule()
        {
            _strategyMapping = new MessageToStrategyMapping<TStrategy>();
        }

        /// <summary>
        /// Returns the mapping of certain messages to specific strategies.
        /// </summary>
        public IMessageToStrategyMapping<TStrategy> StrategyMapping
        {
            get { return _strategyMapping; }
        }

        /// <summary>
        /// The default strategy to use when the message being handled is not explicitly associated with a specific strategy.
        /// </summary>
        protected abstract TStrategy DefaultStrategy
        {
            get;
        }

        /// <inheritdoc />
        public override void Start()
        {
            base.Start();

            _strategyMapping.SwitchToReadOnly();
        }

        /// <summary>
        /// Attempts to retrieve the associated strategy for the message on the specified <paramref name="handler"/>
        /// and then invokes <see cref="InvokeHandler(IMessageHandler, TStrategy)" />.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        protected override void InvokeHandler(IMessageHandler handler)
        {
            TStrategy strategy;

            if (_strategyMapping.TryGetStrategy(handler.Message, out strategy))
            {
                InvokeHandler(handler, strategy);
                return;
            }
            if (Message.TryGetStrategyFromAttribute(handler.Message, out strategy))
            {
                InvokeHandler(handler, strategy);
                return;
            }
            InvokeHandler(handler, DefaultStrategy);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> using the specified <paramref name="strategy"/>.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <param name="strategy">The strategy to use.</param>
        protected abstract void InvokeHandler(IMessageHandler handler, TStrategy strategy);
    }
}
