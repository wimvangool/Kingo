using System.Collections.Generic;

namespace Kingo.Messaging
{
    internal sealed class QueryPipeline : MessageProcessorPipeline<QueryModule>
    {
        internal QueryPipeline(IEnumerable<QueryModule> modules)
            : base(modules) { }

        internal IQueryWrapper<TMessageOut> ConnectTo<TMessageOut>(IQueryWrapper<TMessageOut> query) where TMessageOut : class, IMessage
        {            
            foreach (var module in Modules)
            {
                query = new Query<TMessageOut>(query, module);
            }
            return query;
        }
    }
}
