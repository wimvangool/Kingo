using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a factory that can be used to build a <see cref="IQuery{TMessageIn, TMessageOut}" /> pipeline.
    /// </summary>
    /// <typeparam name="TMessageIn">Type of the messages that go into the pipeline.</typeparam>
    /// <typeparam name="TMessageOut">Type of the messages that come out of the pipeline.</typeparam>
    public class QueryPipelineFactory<TMessageIn, TMessageOut> : IQueryPipelineFactory<TMessageIn, TMessageOut>,
                                                                 IEnumerable<IQueryPipelineFactory<TMessageIn, TMessageOut>>
        where TMessageIn : class, IRequestMessage<TMessageIn>        
    {
        private readonly List<IQueryPipelineFactory<TMessageIn, TMessageOut>> _factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerPipelineFactory{TMessage}" /> class.
        /// </summary>
        public QueryPipelineFactory()
        {
            _factories = new List<IQueryPipelineFactory<TMessageIn, TMessageOut>>();
        }

        /// <summary>
        /// Returns the number of pipelines currently making up the pipeline to be constructed.
        /// </summary>
        public int Count
        {
            get { return _factories.Count; }
        }

        /// <summary>
        /// Adds the specified <paramref name="factory"/> to this factory.
        /// </summary>
        /// <param name="factory">The factory to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>.
        /// </exception>
        public void Add(Func<IQuery<TMessageIn, TMessageOut>, IQuery<TMessageIn, TMessageOut>> factory)
        {
            Add((QueryPipelineFactoryDecorator<TMessageIn, TMessageOut>) factory);
        }

        /// <summary>
        /// Adds the specified <paramref name="factory"/> to this factory.
        /// </summary>
        /// <param name="factory">The factory to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>.
        /// </exception>
        public void Add(IQueryPipelineFactory<TMessageIn, TMessageOut> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            _factories.Add(factory);
        }

        /// <inheritdoc />
        public IQuery<TMessageIn, TMessageOut> CreateQueryPipeline(IQuery<TMessageIn, TMessageOut> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var factoryStack = new Stack<IQueryPipelineFactory<TMessageIn, TMessageOut>>();

            foreach (var factory in _factories)
            {
                factoryStack.Push(factory);
            }
            return CreateMessageHandlerPipeline(handler, factoryStack);
        }

        /// <summary>
        /// Creates and returns a pipeline of <see cref="IQuery{TMessageIn, TMessageOut}">Queries</see>
        /// on top of the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query on which the pipeline is built.</param>
        /// <param name="factoryStack">The factories that are used to build each part of the pipeline.</param>
        /// <returns>The created pipeline</returns>.        
        protected virtual IQuery<TMessageIn, TMessageOut> CreateMessageHandlerPipeline(IQuery<TMessageIn, TMessageOut> query, Stack<IQueryPipelineFactory<TMessageIn, TMessageOut>> factoryStack)
        {
            var pipeline = query;

            while (factoryStack.Count > 0)
            {
                pipeline = factoryStack.Pop().CreateQueryPipeline(pipeline);
            }
            return pipeline;
        }

        #region [====== Enumerable ======]

        IEnumerator<IQueryPipelineFactory<TMessageIn, TMessageOut>> IEnumerable<IQueryPipelineFactory<TMessageIn, TMessageOut>>.GetEnumerator()
        {
            return _factories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _factories.GetEnumerator();
        }

        #endregion
    }
}
