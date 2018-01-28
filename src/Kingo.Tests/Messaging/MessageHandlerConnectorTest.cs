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
            var connector = CreateConnector(handler, new MicroProcessorFilterSpy());

            var result = await connector.HandleAsync(message, MicroProcessorContext.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.OutputStream.Count);
            Assert.AreEqual(0, result.MetadataStream.Count);

            handler.AssertHandleCountIs(1);
            handler.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public async Task HandleAsync_InvokesEmbeddedPipelinesAndHandler_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var connectorA = CreateConnector(handler, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            var result = await connectorB.HandleAsync(message, MicroProcessorContext.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.OutputStream.Count);
            Assert.AreEqual(0, result.MetadataStream.Count);

            handler.AssertHandleCountIs(1);
            handler.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var handler = new MessageHandlerSpy<object>();            
            var connector = CreateConnector(handler, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MessageHandlerSpy<Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var handler = new MessageHandlerSpy<object>();                      
            var connectorA = CreateConnector(handler, new MicroProcessorFilterSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorFilterSpy());

            Assert.AreEqual("MicroProcessorFilterSpy | MicroProcessorFilterSpy | MessageHandlerSpy<Object>", connectorB.ToString());
        }

        private static MessageHandlerPipelineConnector<TMessage> CreateConnector<TMessage>(IMessageHandler<TMessage> handler, IMicroProcessorFilter filter) =>
            CreateConnector(new MessageHandlerDecorator<TMessage>(new MessageHandlerContext(Thread.CurrentPrincipal), handler), filter);

        private static MessageHandlerPipelineConnector<TMessage> CreateConnector<TMessage>(MessageHandler<TMessage> handler, IMicroProcessorFilter filter) =>
            new MessageHandlerPipelineConnector<TMessage>(handler, filter);
    }
}
