using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryFilterPipeline<TMessageOut> : Query<TMessageOut>
    {
        #region [====== ExecuteAsyncMethod ======]

        private sealed class ExecuteAsyncMethod : MessageHandlerOrQueryMethod<TMessageOut>
        {
            private readonly QueryFilterPipeline<TMessageOut> _connector;
            private readonly IMicroProcessorFilter _filter;

            public ExecuteAsyncMethod(QueryFilterPipeline<TMessageOut> connector, IMicroProcessorFilter filter)
            {
                _connector = connector;
                _filter = filter;
            }

            public IMicroProcessorFilter Filter =>
                _filter;

            private MessageHandlerOrQueryMethod<TMessageOut> Method =>
                _connector._nextQuery.Method;

            public override MethodInfo Info =>
                Method.Info;

            public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
                Method.TryGetAttributeOfType(out attribute);

            public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
                Method.GetAttributesOfType<TAttribute>();

            public override Task<InvokeAsyncResult<TMessageOut>> InvokeAsync() =>
                _filter.InvokeQueryAsync(_connector._nextQuery);

            public override string ToString() =>
                Method.ToString();
        }

        #endregion

        private readonly Query<TMessageOut> _nextQuery;
        private readonly ExecuteAsyncMethod _method;

        public QueryFilterPipeline(Query<TMessageOut> nextQuery, IMicroProcessorFilter filter)            
        {
            _nextQuery = nextQuery;
            _method = new ExecuteAsyncMethod(this, filter);
        }
        
        public override string ToString() =>
            $"{_method.Filter.GetType().FriendlyName()} | {_nextQuery}";

        #region [====== IAttributeProvider<Type> ======]

        public override Type Type =>
            _nextQuery.Type;

        public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
            _nextQuery.TryGetAttributeOfType(out attribute);

        public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
            _nextQuery.GetAttributesOfType<TAttribute>();

        #endregion

        #region [====== IMessageHandlerOrQuery<TMessageOut> ======]

        public override QueryContext Context =>
            _nextQuery.Context;

        public override MessageHandlerOrQueryMethod<TMessageOut> Method =>
            _method;

        #endregion
    }
}
