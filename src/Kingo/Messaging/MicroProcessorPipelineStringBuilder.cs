using System.Text;

namespace Kingo.Messaging
{
    internal sealed class MicroProcessorPipelineStringBuilder : IMicroProcessorPipelineVisitor
    {
        private readonly StringBuilder _builder;

        private MicroProcessorPipelineStringBuilder()
        {
            _builder = new StringBuilder();
        }

        public void Visit(IMicroProcessorPipeline pipeline) =>
            _builder.AppendFormat("{0} | ", pipeline.GetType().FriendlyName());

        public void Visit<TMessage>(IMessageHandler<TMessage> handler) =>
            VisitLast(handler);

        public void Visit<TMessageOut>(IQuery<TMessageOut> query) =>
            VisitLast(query);

        public void Visit<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query) =>
            VisitLast(query);

        private void VisitLast(object handlerOrQuery) =>
            _builder.Append(handlerOrQuery.GetType().FriendlyName());

        public override string ToString() =>
            _builder.ToString();

        public static string ToString(IMicroProcessorPipelineComponent component)
        {
            var builder = new MicroProcessorPipelineStringBuilder();
            component.Accept(builder);
            return builder.ToString();
        }
    }
}
