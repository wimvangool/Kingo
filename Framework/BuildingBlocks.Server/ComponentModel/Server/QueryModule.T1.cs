using System;
using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all <see cref="QueryModule">QueryModules</see> that associate
    /// each message with a specific <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <typeparam name="TStrategy">Type of the strategy to associate with each message.</typeparam>
    public abstract class QueryModule<TStrategy> : QueryModule where TStrategy : class
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
        /// Attempts to retrieve the associated strategy for the message on the specified <paramref name="query"/>
        /// and then invokes <see cref="InvokeAsync{TMessageOut}(IQuery{TMessageOut}, TStrategy)" />.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <returns>A task executing the query.</returns>
        public override Task<TMessageOut> InvokeAsync<TMessageOut>(IQuery<TMessageOut> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            TStrategy strategy;

            if (ClassAndMethodAttributeProvider.TryGetStrategyFromAttribute(query, out strategy))
            {
                return InvokeAsync(query, strategy);
            }
            if (StrategyMapping.TryGetStrategy(query.MessageIn, out strategy))
            {
                return InvokeAsync(query, strategy);
            }
            if (Message.TryGetStrategyFromAttribute(query.MessageIn, out strategy))
            {
                return InvokeAsync(query, strategy);
            }
            return InvokeAsync(query, DefaultStrategy);
        }

        /// <summary>
        /// Invokes the specified <paramref name="query"/> using the specified <paramref name="strategy"/>.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <param name="strategy">The strategy to use.</param>
        /// <returns>A task executing the query.</returns>
        protected abstract Task<TMessageOut> InvokeAsync<TMessageOut>(IQuery<TMessageOut> query, TStrategy strategy) where TMessageOut : class, IMessage<TMessageOut>;
    }
}
