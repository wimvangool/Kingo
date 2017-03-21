using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryWrapper<TMessageOut> : MessageHandlerOrQuery<ExecuteAsyncResult<TMessageOut>>
    {
        private readonly Query<TMessageOut> _query;

        public QueryWrapper(Query<TMessageOut> query)
        {
            _query = query;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _query;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
             _query;

        public override Task<ExecuteAsyncResult<TMessageOut>> HandleMessageOrExecuteQueryAsync(IMicroProcessorContext context) =>
            _query.ExecuteAsync(context);

        public override string ToString() =>
            _query.ToString();
    }
}
