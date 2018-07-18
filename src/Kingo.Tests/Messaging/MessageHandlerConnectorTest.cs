using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class MessageHandlerConnectorTest
    {        
        [TestMethod]
        public async Task HandleAsync_InvokesEmbeddedPipelineAndHandler_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var connector = CreateConnector(handler, message, new MicroProcessorFilterSpy());

            var result = await connector.InvokeAsync(CreateProcessorContext());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value.Count);            

            handler.AssertHandleCountIs(1);
            handler.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public async Task HandleAsync_InvokesEmbeddedPipelinesAndHandler_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var connectorA = CreateConnector(handler, message, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            var result = await connectorB.InvokeAsync(CreateProcessorContext());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Value.Count);            

            handler.AssertHandleCountIs(1);
            handler.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var connector = CreateConnector(handler, message, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MessageHandlerSpy<Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();                      
            var connectorA = CreateConnector(handler, message, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | MessageHandlerSpy<Object>", connectorB.ToString());
        }

        private static MessageHandlerConnector CreateConnector<TMessage>(IMessageHandler<TMessage> handler, TMessage message, IMicroProcessorFilter filter) =>
            CreateConnector(new MessageHandlerDecorator<TMessage>(handler, message), filter);

        private static MessageHandlerConnector CreateConnector(MessageHandler handler, IMicroProcessorFilter filter) =>
            new MessageHandlerConnector(handler, filter);

        private static MicroProcessorContext CreateProcessorContext() =>
            new MessageHandlerContext(Thread.CurrentPrincipal);
    }
}
