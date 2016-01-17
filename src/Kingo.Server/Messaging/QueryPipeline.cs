using System.Collections.Generic;

namespace Kingo.Messaging
{
    internal sealed class QueryPipeline : MessageProcessorPipeline<QueryModule>
    {
        internal QueryPipeline(IEnumerable<QueryModule> modules)
            : base(modules) { }

        internal IQuery<TMessageOut> ConnectTo<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage
        {            
            foreach (var module in Modules)
            {
                query = new Query<TMessageOut>(query, module);
            }
            return query;
        }
    }
}
