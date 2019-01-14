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
            var pipeline = CreateFilterPipeline(query, message, CreateFilter());

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
            var pipelineA = CreateFilterPipeline(query, message, CreateFilter());
            var pipelineB = CreateFilterPipeline(pipelineA, CreateFilter());

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
            var connector = CreateFilterPipeline(query, message, CreateFilter());

            Assert.AreEqual("ExecuteAsync(Object, QueryContext)", connector.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpyAttribute | QuerySpy<Object, Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connectorA = CreateFilterPipeline(query, message, CreateFilter());
            var connectorB = CreateFilterPipeline(connectorA, CreateFilter());

            Assert.AreEqual("ExecuteAsync(Object, QueryContext)", connectorB.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpyAttribute | MicroProcessorFilterSpyAttribute | QuerySpy<Object, Object>", connectorB.ToString());
        }        

        private static QueryFilterPipeline<TResponse> CreateFilterPipeline<TRequest, TResponse>(IQuery<TRequest, TResponse> query, TRequest message, IMicroProcessorFilter filter) =>
            CreateFilterPipeline(new QueryDecorator<TRequest, TResponse>(query, message, CreateQueryContext(message)), filter);

        private static QueryFilterPipeline<TResponse> CreateFilterPipeline<TResponse>(Query<TResponse> query, IMicroProcessorFilter filter) =>
            new QueryFilterPipeline<TResponse>(query, filter);

        private static QueryContext CreateQueryContext(object message) =>
            new QueryContext(new NullServiceProvider(), Thread.CurrentPrincipal, null, message);

        private static IMicroProcessorFilter CreateFilter() =>
            new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage);
    }
}
