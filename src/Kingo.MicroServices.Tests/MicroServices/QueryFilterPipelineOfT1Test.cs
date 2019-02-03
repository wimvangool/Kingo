using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class QueryFilterPipelineOfT1Test
    {
        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelineAndQuery_IfOnlyOnePipelineIsUsed()
        {            
            var query = new QuerySpy<object>();
            var pipeline = CreateFilterPipeline(query, CreateFilter());

            var result = await pipeline.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.IsNull(result.GetValue());            

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var pipelineA = CreateFilterPipeline(query, CreateFilter());
            var pipelineB = CreateFilterPipeline(pipelineA, CreateFilter());

            var result = await pipelineB.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.IsNull(result.GetValue());            

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var query = new QuerySpy<object>();
            var pipeline = CreateFilterPipeline(query, CreateFilter());

            Assert.AreEqual("ExecuteAsync(QueryContext)", pipeline.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpyAttribute | QuerySpy<Object>", pipeline.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var pipelineA = CreateFilterPipeline(query, CreateFilter());
            var pipelineB = CreateFilterPipeline(pipelineA, CreateFilter());

            Assert.AreEqual("ExecuteAsync(QueryContext)", pipelineB.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpyAttribute | MicroProcessorFilterSpyAttribute | QuerySpy<Object>", pipelineB.ToString());
        }        

        private static QueryFilterPipeline<TResponse> CreateFilterPipeline<TResponse>(IQuery<TResponse> query, IMicroProcessorFilter filter) =>
            CreateFilterPipeline(new QueryDecorator<TResponse>(query, CreateQueryContext()), filter);

        private static QueryFilterPipeline<TResponse> CreateFilterPipeline<TResponse>(Query<TResponse> query, IMicroProcessorFilter filter) =>
            new QueryFilterPipeline<TResponse>(query, filter);

        private static QueryContext CreateQueryContext() =>
            new QueryContext(ServiceProvider.Default, Thread.CurrentPrincipal, null);

        private static IMicroProcessorFilter CreateFilter() =>
            new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage);
    }
}
