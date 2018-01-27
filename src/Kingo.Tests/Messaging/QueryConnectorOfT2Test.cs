using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class QueryConnectorOfT2Test
    {
        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelineAndQuery_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connector = CreateConnector(query, new MicroProcessorFilterSpy());

            var result = await connector.ExecuteAsync(message, MicroProcessorContext.None);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Message);
            Assert.AreEqual(0, result.MetadataStream.Count);

            query.AssertExecuteCountIs(1);
            query.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connectorA = CreateConnector(query, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            var result = await connectorB.ExecuteAsync(message, MicroProcessorContext.None);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Message);
            Assert.AreEqual(0, result.MetadataStream.Count);

            query.AssertExecuteCountIs(1);
            query.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var query = new QuerySpy<object, object>();
            var connector = CreateConnector(query, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | QuerySpy<Object, Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object, object>();
            var connectorA = CreateConnector(query, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | QuerySpy<Object, Object>", connectorB.ToString());
        }

        private static QueryPipelineConnector<TMessageIn, TMessageOut> CreateConnector<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, IMicroProcessorFilter filter) =>
            CreateConnector(new QueryDecorator<TMessageIn, TMessageOut>(new QueryContext(), query), filter);

        private static QueryPipelineConnector<TMessageIn, TMessageOut> CreateConnector<TMessageIn, TMessageOut>(Query<TMessageIn, TMessageOut> query, IMicroProcessorFilter filter) =>
            new QueryPipelineConnector<TMessageIn, TMessageOut>(query, filter);
    }
}
