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
            var connector = CreateConnector(handler, new MicroProcessorPipelineSpy());

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
            var connectorA = CreateConnector(handler, new MicroProcessorPipelineSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorPipelineSpy());

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
            var connector = CreateConnector(handler, new MicroProcessorPipelineSpy());

            Assert.AreEqual("MicroProcessorPipelineSpy | MessageHandlerSpy<Object>", connector.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var handler = new MessageHandlerSpy<object>();                      
            var connectorA = CreateConnector(handler, new MicroProcessorPipelineSpy());
            var connectorB = CreateConnector(connectorA, new MicroProcessorPipelineSpy());

            Assert.AreEqual("MicroProcessorPipelineSpy | MicroProcessorPipelineSpy | MessageHandlerSpy<Object>", connectorB.ToString());
        }

        private static MessageHandlerConnector<TMessage> CreateConnector<TMessage>(IMessageHandler<TMessage> handler, IMicroProcessorPipeline pipeline) =>
            CreateConnector(new MessageHandlerDecorator<TMessage>(new MessageHandlerContext(), handler), pipeline);

        private static MessageHandlerConnector<TMessage> CreateConnector<TMessage>(MessageHandler<TMessage> handler, IMicroProcessorPipeline pipeline) =>
            new MessageHandlerConnector<TMessage>(handler, pipeline);
    }
}
