using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class QueryFilterPipeline<TResponse> : Query<TResponse>
    {
        #region [====== ExecuteAsyncMethod ======]

        private sealed class ExecuteAsyncMethod : MessageHandlerOrQueryMethod<TResponse>
        {
            private readonly QueryFilterPipeline<TResponse> _connector;
            private readonly IMicroProcessorFilter _filter;

            public ExecuteAsyncMethod(QueryFilterPipeline<TResponse> connector, IMicroProcessorFilter filter)
            {
                _connector = connector;
                _filter = filter;
            }

            public IMicroProcessorFilter Filter =>
                _filter;

            private MessageHandlerOrQueryMethod<TResponse> Method =>
                _connector._nextQuery.Method;

            public override MethodInfo Info =>
                Method.Info;

            public override bool TryGetAttributeOfType<TAttribute>(out TAttribute attribute) =>
                Method.TryGetAttributeOfType(out attribute);

            public override IEnumerable<TAttribute> GetAttributesOfType<TAttribute>() =>
                Method.GetAttributesOfType<TAttribute>();

            public override Task<InvokeAsyncResult<TResponse>> InvokeAsync() =>
                _filter.InvokeQueryAsync(_connector._nextQuery);

            public override string ToString() =>
                Method.ToString();
        }

        #endregion

        private readonly Query<TResponse> _nextQuery;
        private readonly ExecuteAsyncMethod _method;

        public QueryFilterPipeline(Query<TResponse> nextQuery, IMicroProcessorFilter filter)            
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

        #region [====== IMessageHandlerOrQuery<TResponse> ======]

        public override QueryContext Context =>
            _nextQuery.Context;

        public override MessageHandlerOrQueryMethod<TResponse> Method =>
            _method;

        #endregion
    }
}
