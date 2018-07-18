using System.Threading;
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
            var connector = CreateConnector(query, message, new MicroProcessorFilterSpy());

            var result = await connector.InvokeAsync(CreateProcessorContext());

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);            

            query.AssertExecuteCountIs(1);
            query.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connectorA = CreateConnector(query, message, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            var result = await connectorB.InvokeAsync(CreateProcessorContext());

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);            

            query.AssertExecuteCountIs(1);
            query.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connector = CreateConnector(query, message, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | QuerySpy<Object, Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var query = new QuerySpy<object, object>();
            var connectorA = CreateConnector(query, message, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | QuerySpy<Object, Object>", connectorB.ToString());
        }        

        private static QueryConnector<TMessageOut> CreateConnector<TMessageIn, TMessageOut>(IQuery<TMessageIn, TMessageOut> query, TMessageIn message, IMicroProcessorFilter filter) =>
            CreateConnector(new QueryDecorator<TMessageIn, TMessageOut>(query, message), filter);

        private static QueryConnector<TMessageOut> CreateConnector<TMessageOut>(Query<TMessageOut> query, IMicroProcessorFilter filter) =>
            new QueryConnector<TMessageOut>(query, filter);

        private static MicroProcessorContext CreateProcessorContext() =>
            new QueryContext(Thread.CurrentPrincipal);
    }
}
