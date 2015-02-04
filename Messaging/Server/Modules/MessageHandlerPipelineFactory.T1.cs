using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Represents a factory that can be used to build a <see cref="IMessageHandler{TMessage}" /> pipeline.
    /// </summary>
    /// <typeparam name="TMessage">Type of the messages that are handled by the pipeline.</typeparam>
    public class MessageHandlerPipelineFactory<TMessage> : IMessageHandlerPipelineFactory<TMessage>,
                                                           IEnumerable<IMessageHandlerPipelineFactory<TMessage>>
        where TMessage : class
    {
        private readonly List<IMessageHandlerPipelineFactory<TMessage>> _factories;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerPipelineFactory{TMessage}" /> class.
        /// </summary>
        public MessageHandlerPipelineFactory()
        {
            _factories = new List<IMessageHandlerPipelineFactory<TMessage>>();
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
        public void Add(Func<IMessageHandler<TMessage>, IMessageHandler<TMessage>> factory)
        {
            Add((MessageHandlerPipelineFactoryDecorator<TMessage>) factory);
        }

        /// <summary>
        /// Adds the specified <paramref name="factory"/> to this factory.
        /// </summary>
        /// <param name="factory">The factory to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> is <c>null</c>.
        /// </exception>
        public void Add(IMessageHandlerPipelineFactory<TMessage> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }
            _factories.Add(factory);
        }

        /// <inheritdoc />
        public IMessageHandler<TMessage> CreateMessageHandlerPipeline(IMessageHandler<TMessage> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            var factoryStack = new Stack<IMessageHandlerPipelineFactory<TMessage>>();

            foreach (var factory in _factories)
            {
                factoryStack.Push(factory);
            }
            return CreateMessageHandlerPipeline(handler, factoryStack);
        }

        /// <summary>
        /// Creates and returns a pipeline of <see cref="IMessageHandler{TMessage}">MessageHandlers</see>
        /// on top of the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The handler on which the pipeline is built.</param>
        /// <param name="factoryStack">The factories that are used to build each part of the pipeline.</param>
        /// <returns>The created pipeline</returns>.        
        protected virtual IMessageHandler<TMessage> CreateMessageHandlerPipeline(IMessageHandler<TMessage> handler, Stack<IMessageHandlerPipelineFactory<TMessage>> factoryStack)
        {
            var pipeline = handler;

            while (factoryStack.Count > 0)
            {
                pipeline = factoryStack.Pop().CreateMessageHandlerPipeline(pipeline);
            }
            return pipeline;
        }

        #region [====== Enumerable ======]

        IEnumerator<IMessageHandlerPipelineFactory<TMessage>> IEnumerable<IMessageHandlerPipelineFactory<TMessage>>.GetEnumerator()
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
