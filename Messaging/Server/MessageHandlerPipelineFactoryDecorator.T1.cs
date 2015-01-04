namespace System.ComponentModel.Server
{
    /// <summary>
    /// This type is used to support implicit type conversion from a <see cref="Func{T, S}" /> to a
    /// <see cref="IMessageHandlerPipelineFactory{TMessage}" />.
    /// </summary>
    /// <typeparam name="TMessage">Type of the message that is handled by this handler.</typeparam>
    public sealed class MessageHandlerPipelineFactoryDecorator<TMessage> : IMessageHandlerPipelineFactory<TMessage> where TMessage : class
    {
        private readonly Func<IMessageHandler<TMessage>, IMessageHandler<TMessage>> _factory;

        private MessageHandlerPipelineFactoryDecorator(Func<IMessageHandler<TMessage>, IMessageHandler<TMessage>> factory)
        {            
            _factory = factory;
        }

        IMessageHandler<TMessage> IMessageHandlerPipelineFactory<TMessage>.CreateMessageHandlerPipeline(IMessageHandler<TMessage> handler)
        {            
            return _factory.Invoke(handler);
        }

        /// <summary>
        /// Implicitly converts <paramref name="factory"/> to an instance of <see cref="MessageHandlerPipelineFactoryDecorator{TMessage}" />.
        /// </summary>
        /// <param name="factory">The value to convert.</param>
        /// <returns>
        /// <c>null</c> if <paramref name="factory"/> is <c>null</c>;
        /// otherwise, a new instance of <see cref="MessageHandlerPipelineFactoryDecorator{TMessage}" />.
        /// </returns>
        public static implicit operator MessageHandlerPipelineFactoryDecorator<TMessage>(Func<IMessageHandler<TMessage>, IMessageHandler<TMessage>> factory)
        {
            return factory == null ? null : new MessageHandlerPipelineFactoryDecorator<TMessage>(factory);
        }
    }
}
