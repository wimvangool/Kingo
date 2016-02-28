using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryDispatcherModule<TMessageOut> : MessageHandlerDispatcher        
        where TMessageOut : class, IMessage
    {
        private readonly IQueryWrapper<TMessageOut> _query;        
        private readonly MessageProcessor _processor;

        internal QueryDispatcherModule(IQueryWrapper<TMessageOut> query, MessageProcessor processor)
        {
            _query = query;
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
            return await _processor.BuildQueryExecutionPipeline().ConnectTo(_query).ExecuteAsync();            
        }        
    }
}
