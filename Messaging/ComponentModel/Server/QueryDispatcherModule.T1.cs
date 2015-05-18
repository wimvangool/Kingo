namespace System.ComponentModel.Server
{
    internal sealed class QueryDispatcherModule<TMessageIn, TMessageOut> : IMessageHandler
        where TMessageIn : class, IMessage<TMessageIn>
        where TMessageOut : class, IMessage<TMessageOut>
    {
        private readonly IQuery<TMessageOut> _query;        
        private readonly MessageProcessor _processor;

        internal QueryDispatcherModule(TMessageIn message, IQuery<TMessageIn, TMessageOut> query, MessageProcessor processor)
        {
            _query = new QueryWrapper<TMessageIn, TMessageOut>(message, query);
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
            _processor.Message.ThrowIfCancellationRequested();

            using (var scope = _processor.CreateUnitOfWorkScope())
            {
                MessageOut = ExecuteQuery();

                scope.Complete();
            }
            _processor.Message.ThrowIfCancellationRequested();
        }

        private TMessageOut ExecuteQuery()
        {            
            var messageOut = _processor.BuildQueryExecutionPipeline().ConnectTo(_query).Invoke();

            _processor.Message.ThrowIfCancellationRequested();

            return messageOut;
        }        
    }
}
