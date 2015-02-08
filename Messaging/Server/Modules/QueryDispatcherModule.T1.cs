namespace System.ComponentModel.Server.Modules
{
    internal sealed class QueryDispatcherModule<TMessageIn, TMessageOut> : IMessageHandler<TMessageIn> where TMessageIn : class, IMessage<TMessageIn>
    {
        private readonly IQuery<TMessageIn, TMessageOut> _query;
        private readonly MessageProcessor _processor;

        internal QueryDispatcherModule(IQuery<TMessageIn, TMessageOut> query, MessageProcessor processor)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }
            _query = query;
            _processor = processor;
        }

        internal TMessageOut Result
        {
            get;
            private set;
        }

        void IMessageHandler<TMessageIn>.Handle(TMessageIn message)
        {
            _processor.MessagePointer.ThrowIfCancellationRequested();

            using (var scope = new UnitOfWorkScope(_processor.DomainEventBus))
            {
                Result = Execute(message);

                scope.Complete();
            }
            _processor.MessagePointer.ThrowIfCancellationRequested();
        }

        private TMessageOut Execute(TMessageIn message)
        {
            var result = _processor.CreateQueryPipeline<TMessageIn, TMessageOut>().CreateQueryPipeline(_query).Execute(message);

            _processor.MessagePointer.ThrowIfCancellationRequested();

            return result;
        }
    }
}
