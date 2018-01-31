using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal sealed class QueryWrapper<TMessageIn, TMessageOut> : MessageHandlerOrQuery<ExecuteAsyncResult<TMessageOut>>
    {
        private readonly Query<TMessageIn, TMessageOut> _query;
        private readonly TMessageIn _message;

        public QueryWrapper(Query<TMessageIn, TMessageOut> query, TMessageIn message)
        {
            _query = query;
            _message = message;
        }

        protected override ITypeAttributeProvider TypeAttributeProvider =>
            _query;

        protected override IMethodAttributeProvider MethodAttributeProvider =>
            _query;

        public override Task<ExecuteAsyncResult<TMessageOut>> InvokeAsync(IMicroProcessorContext context) =>
            _query.ExecuteAsync(_message, context);

        public override string ToString() =>
            _query.ToString();
    }
}
