using System.Security.Principal;
using System.Threading;
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

            var result = await connector.InvokeAsync(CreateProcessorContext());

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);            

            query.AssertExecuteCountIs(1);
        }

        [TestMethod]
        public async Task ExecuteAsync_InvokesEmbeddedPipelinesAndQuery_IfManyPipelinesAreUsed()
        {
            var query = new QuerySpy<object>();
            var connectorA = CreateConnector(query, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            var result = await connectorB.InvokeAsync(CreateProcessorContext());

            Assert.IsNotNull(result);
            Assert.IsNull(result.Value);            

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

        private static IPrincipal Principal =>
            Thread.CurrentPrincipal;

        private static QueryConnector<TMessageOut> CreateConnector<TMessageOut>(IQuery<TMessageOut> query, IMicroProcessorFilter filter) =>
            CreateConnector(new QueryDecorator<TMessageOut>(query), filter);

        private static QueryConnector<TMessageOut> CreateConnector<TMessageOut>(Query<TMessageOut> query, IMicroProcessorFilter filter) =>
            new QueryConnector<TMessageOut>(query, filter);

        private static MicroProcessorContext CreateProcessorContext() =>
            new QueryContext(Principal);
    }
}
