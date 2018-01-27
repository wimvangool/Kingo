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
            var connector = CreateConnector(query, new MicroProcessorFilterSpy());

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
            var connectorA = CreateConnector(query, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

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
            var connector = CreateConnector(query, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | QuerySpy<Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var connectorA = CreateConnector(query, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | QuerySpy<Object>", connectorB.ToString());
        }

        private static QueryPipelineConnector<TMessageOut> CreateConnector<TMessageOut>(IQuery<TMessageOut> query, IMicroProcessorFilter filter) =>
            CreateConnector(new QueryDecorator<TMessageOut>(new QueryContext(), query), filter);

        private static QueryPipelineConnector<TMessageOut> CreateConnector<TMessageOut>(Query<TMessageOut> query, IMicroProcessorFilter filter) =>
            new QueryPipelineConnector<TMessageOut>(query, filter);
    }
}
