using System;
using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server
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
        /// and then invokes <see cref="InvokeAsync(IMessageHandler, TStrategy)" />.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <returns>A task carrying out the invocation.</returns>
        public override Task InvokeAsync(IMessageHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            TStrategy strategy;

            if (ClassAndMethodAttributeProvider.TryGetStrategyFromAttribute(handler, out strategy))
            {
                return InvokeAsync(handler, strategy);
            }
            if (StrategyMapping.TryGetStrategy(handler.Message, out strategy))
            {
                return InvokeAsync(handler, strategy);                
            }
            if (Message.TryGetStrategyFromAttribute(handler.Message, out strategy))
            {
                return InvokeAsync(handler, strategy);                
            }
            return InvokeAsync(handler, DefaultStrategy);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/> using the specified <paramref name="strategy"/>.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <param name="strategy">The strategy to use.</param>
        /// <returns>A task carrying out the invocation.</returns>
        protected abstract Task InvokeAsync(IMessageHandler handler, TStrategy strategy);         
    }
}
