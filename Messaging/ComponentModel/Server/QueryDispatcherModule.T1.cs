namespace System.ComponentModel.Server
{
    internal sealed class QueryDispatcherModule<TMessageIn, TMessageOut> : IMessageHandler
        where TMessageIn : class, IMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IQuery<TMessageOut> _query;        
        private readonly MessageProcessor _processor;

        internal QueryDispatcherModule(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, QueryExecutionOptions options, MessageProcessor processor)
        {
            _query = new QueryWrapper<TMessageIn, TMessageOut>(message, query, options);
            _processor = processor;
        }

        IMessage IMessageHandler.Message
        {
            get { return _query.MessageIn; }
        }

        internal TMessageOut MessageOut
        {
            get;
            private set;
        }

        void IMessageHandler.Invoke()
        {
            _processor.MessagePointer.ThrowIfCancellationRequested();

            using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
            {
                MessageOut = ExecuteQuery();

                scope.Complete();
            }
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }

        private TMessageOut ExecuteQuery()
        {
            var pipeline = _processor.DataAccessPipeline.ConnectTo(_query);
            var messageOut = pipeline.Invoke();

            _processor.MessagePointer.ThrowIfCancellationRequested();

            return messageOut;
        }        
    }
}
