using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Serves as a base class for all <see cref="QueryModule">QueryModules</see> that associate
    /// each message with a specific <typeparamref name="TStrategy"/>.
    /// </summary>
    /// <typeparam name="TStrategy">Type of the strategy to associate with each message.</typeparam>
    public abstract class QueryModule<TStrategy> : QueryModule where TStrategy : class
    {
        private readonly MessageToStrategyMapping<TStrategy> _strategyMapping;        

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryModule" /> class.
        /// </summary>               
        protected QueryModule()
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

        /// <summary>
        /// Attempts to retrieve the associated strategy for the message on the specified <paramref name="query"/>
        /// and then invokes <see cref="InvokeQuery{TMessageOut}(IQuery{TMessageOut}, TStrategy)" />.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        protected override TMessageOut InvokeQuery<TMessageOut>(IQuery<TMessageOut> query)
        {
            TStrategy strategy;

            if (StrategyMapping.TryGetStrategy(query.MessageIn, out strategy))
            {
                return InvokeQuery(query, strategy);
            }
            if (Message.TryGetStrategyFromAttribute(query.MessageIn, out strategy))
            {
                return InvokeQuery(query, strategy);
            }
            return InvokeQuery(query, DefaultStrategy);
        }

        /// <summary>
        /// Invokes the specified <paramref name="query"/> using the specified <paramref name="strategy"/>.
        /// </summary>
        /// <param name="query">The query to invoke.</param>
        /// <param name="strategy">The strategy to use.</param>
        protected abstract TMessageOut InvokeQuery<TMessageOut>(IQuery<TMessageOut> query, TStrategy strategy) where TMessageOut : class, IMessage<TMessageOut>;
    }
}
