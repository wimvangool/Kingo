using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorPipelineFactory : IMicroProcessorPipelineFactory
    {
        #region [====== NullFactory ======]

        private sealed class NullPipelineFactory : IMicroProcessorPipelineFactory
        {
            public MessageHandler CreatePipeline(MessageHandler handler) =>
                handler ?? throw new ArgumentNullException(nameof(handler));

            public Query<TMessageOut> CreatePipeline<TMessageOut>(Query<TMessageOut> query) =>
                query ?? throw new ArgumentNullException(nameof(query));
        }

        public static readonly IMicroProcessorPipelineFactory Null = new NullPipelineFactory();

        #endregion

        private readonly IMicroProcessorPipelineFactory[] _parts;

        public MicroProcessorPipelineFactory(IEnumerable<IMicroProcessorPipelineFactory> parts)
        {
            _parts = parts.Reverse().ToArray();
        }

        public MessageHandler CreatePipeline(MessageHandler handler)
        {
            foreach (var factory in _parts)
            {
                handler = factory.CreatePipeline(handler);
            }
            return handler;
        }

        public Query<TMessageOut> CreatePipeline<TMessageOut>(Query<TMessageOut> query)
        {
            foreach (var factory in _parts)
            {
                query = factory.CreatePipeline(query);
            }
            return query;
        }
    }
}
