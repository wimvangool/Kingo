using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class QueryConnectorOfT1Test
    {
        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelineAndQuery_IfOnlyOnePipelineIsUsed()
        {            
            var query = new QuerySpy<object>();
            var connector = CreateConnector(query, new MicroProcessorPipelineSpy());

            var result = await connector.ExecuteAsync(MicroProcessorContext.None);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Message);
            Assert.AreEqual(0, result.MetadataStream.Count);

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var connectorA = CreateConnector(query, new MicroProcessorPipelineSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorPipelineSpy());

            var result = await connectorB.ExecuteAsync(MicroProcessorContext.None);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Message);
            Assert.AreEqual(0, result.MetadataStream.Count);

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var query = new QuerySpy<object>();
            var connector = CreateConnector(query, new MicroProcessorPipelineSpy());

            Assert.AreEqual("MicroProcessorPipelineSpy | QuerySpy<Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var connectorA = CreateConnector(query, new MicroProcessorPipelineSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorPipelineSpy());

            Assert.AreEqual("MicroProcessorPipelineSpy | MicroProcessorPipelineSpy | QuerySpy<Object>", connectorB.ToString());
        }

        private static QueryConnector<TMessageOut> CreateConnector<TMessageOut>(IQuery<TMessageOut> query, IMicroProcessorPipeline pipeline) =>
            CreateConnector(new QueryDecorator<TMessageOut>(new QueryContext(), query), pipeline);

        private static QueryConnector<TMessageOut> CreateConnector<TMessageOut>(Query<TMessageOut> query, IMicroProcessorPipeline pipeline) =>
            new QueryConnector<TMessageOut>(query, pipeline);
    }
}
