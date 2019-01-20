using System.Collections.Generic;
using System.Linq;
using Kingo.MicroServices.Configuration;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorPipelineFactory : IMicroProcessorPipelineFactory
    {        
        public static readonly IMicroProcessorPipelineFactory Null = new MicroProcessorPipelineFactoryBuilder().Build();        

        private readonly IMicroProcessorPipelineFactory[] _parts;

        public MicroProcessorPipelineFactory(IEnumerable<IMicroProcessorPipelineFactory> parts)
        {
            _parts = parts.Reverse().ToArray();
        }

        public override string ToString() =>
            GetType().FriendlyName();

        public MessageHandler CreatePipeline(MessageHandler handler)
        {
            foreach (var factory in _parts)
            {
                handler = factory.CreatePipeline(handler);
            }
            return handler;
        }

        public Query<TResponse> CreatePipeline<TResponse>(Query<TResponse> query)
        {
            foreach (var factory in _parts)
            {
                query = factory.CreatePipeline(query);
            }
            return query;
        }
    }
}
