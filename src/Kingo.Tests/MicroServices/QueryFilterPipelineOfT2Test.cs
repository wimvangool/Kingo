using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class QueryFilterPipelineOfT2Test
    {
        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelineAndQuery_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var pipeline = CreateFilterPipeline(query, message, new MicroProcessorFilterSpy());

            var result = await pipeline.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.IsNull(result.GetValue());            

            query.AssertExecuteCountIs(1);
            query.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var pipelineA = CreateFilterPipeline(query, message, new MicroProcessorFilterSpy());
            var pipelineB = CreateFilterPipeline(pipelineA, new MicroProcessorFilterSpy());

            var result = await pipelineB.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.IsNull(result.GetValue());            

            query.AssertExecuteCountIs(1);
            query.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connector = CreateFilterPipeline(query, message, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | QuerySpy<Object, Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connectorA = CreateFilterPipeline(query, message, new MicroProcessorFilterSpy());
            var connectorB = CreateFilterPipeline(connectorA, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | QuerySpy<Object, Object>", connectorB.ToString());
        }        

        private static QueryFilterPipeline<TMessageOut> CreateFilterPipeline<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorFilter filter) =>
            CreateFilterPipeline(new QueryDecorator<TMessageIn, TMessageOut>(query, message, CreateQueryContext(message)), filter);

        private static QueryFilterPipeline<TMessageOut> CreateFilterPipeline<TMessageOut>(Query<TMessageOut> query, IMicroProcessorFilter filter) =>
            new QueryFilterPipeline<TMessageOut>(query, filter);

        private static QueryContext CreateQueryContext(object message) =>
            new QueryContext(new SimpleServiceProvider(), Thread.CurrentPrincipal, null, message);
    }
}
