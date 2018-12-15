using System.Security.Principal;
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
            var pipeline = CreateFilterPipeline(query, new MicroProcessorFilterSpy());

            var result = await pipeline.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.IsNull(result.GetValue());            

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var pipelineA = CreateFilterPipeline(query, new MicroProcessorFilterSpy());
            var pipelineB = CreateFilterPipeline(pipelineA, new MicroProcessorFilterSpy());

            var result = await pipelineB.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.IsNull(result.GetValue());            

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var query = new QuerySpy<object>();
            var pipeline = CreateFilterPipeline(query, new MicroProcessorFilterSpy());

            Assert.AreEqual("ExecuteAsync(QueryContext)", pipeline.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpy | QuerySpy<Object>", pipeline.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var pipelineA = CreateFilterPipeline(query, new MicroProcessorFilterSpy());
            var pipelineB = CreateFilterPipeline(pipelineA, new MicroProcessorFilterSpy());

            Assert.AreEqual("ExecuteAsync(QueryContext)", pipelineB.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | QuerySpy<Object>", pipelineB.ToString());
        }        

        private static QueryFilterPipeline<TMessageOut> CreateFilterPipeline<TMessageOut>(IQuery<TMessageOut> query, IMicroProcessorFilter filter) =>
            CreateFilterPipeline(new QueryDecorator<TMessageOut>(query, CreateQueryContext()), filter);

        private static QueryFilterPipeline<TMessageOut> CreateFilterPipeline<TMessageOut>(Query<TMessageOut> query, IMicroProcessorFilter filter) =>
            new QueryFilterPipeline<TMessageOut>(query, filter);

        private static QueryContext CreateQueryContext() =>
            new QueryContext(new NullServiceProvider(), Thread.CurrentPrincipal, null);
    }
}
