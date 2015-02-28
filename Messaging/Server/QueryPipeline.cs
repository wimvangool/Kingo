using System.Collections.Generic;

namespace System.ComponentModel.Server
{
    internal sealed class QueryPipeline : MessageProcessorPipeline<IQueryModule>
    {
        internal QueryPipeline(IEnumerable<IQueryModule> modules)
            : base(modules) { }

        internal IQuery<TMessageOut> ConnectTo<TMessageOut>(IQuery<TMessageOut> query) where TMessageOut : class, IMessage<TMessageOut>
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            foreach (var module in Modules)
            {
                query = new Query<TMessageOut>(query, module);
            }
            return query;
        }
    }
}
