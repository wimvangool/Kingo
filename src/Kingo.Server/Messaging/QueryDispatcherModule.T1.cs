using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryDispatcherModule<TMessageIn, TMessageOut> : MessageHandlerDispatcher
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

        public override IMessage Message
        {
            get { return _query.MessageIn; }
        }

        internal TMessageOut MessageOut
        {
            get;
            private set;
        }

        public override async Task InvokeAsync()
        {
            ThrowIfCancellationRequested();

            using (var scope = UnitOfWorkContext.StartUnitOfWorkScope(_processor))
            {
                MessageOut = await ExecuteQueryAsync();

                await scope.CompleteAsync();
            }
            ThrowIfCancellationRequested();
        }        

        private async Task<TMessageOut> ExecuteQueryAsync()
        {            
            return await _processor.BuildQueryExecutionPipeline().ConnectTo(_query).InvokeAsync();            
        }        
    }
}
