using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class MessageHandlerFilterPipelineTest
    {        
        [TestMethod]
        public async Task HandleAsync_InvokesEmbeddedPipelineAndHandler_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var pipeline = CreateFilterPipeline(handler, message, CreateFilter());

            var result = await pipeline.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.GetValue().Count);            

            handler.AssertHandleCountIs(1);
            handler.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public async Task HandleAsync_InvokesEmbeddedPipelinesAndHandler_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var pipelineA = CreateFilterPipeline(handler, message, CreateFilter());
            var pipelineB = CreateFilterPipeline(pipelineA, CreateFilter());

            var result = await pipelineB.Method.InvokeAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.GetValue().Count);            

            handler.AssertHandleCountIs(1);
            handler.AssertMessageReceived(0, message);
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfOnlyOnePipelineIsUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();            
            var pipeline = CreateFilterPipeline(handler, message, CreateFilter());

            Assert.AreEqual("HandleAsync(Object, MessageHandlerContext)", pipeline.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpyAttribute | MessageHandlerSpy<Object>", pipeline.ToString());
        }

        [TestMethod]
        public void ToString_ReturnsExpectedValue_IfManyPipelinesAreUsed()
        {
            var message = new object();
            var handler = new MessageHandlerSpy<object>();                      
            var pipelineA = CreateFilterPipeline(handler, message, CreateFilter());
            var pipelineB = CreateFilterPipeline(pipelineA, CreateFilter());

            Assert.AreEqual("HandleAsync(Object, MessageHandlerContext)", pipelineB.Method.ToString());
            Assert.AreEqual("MicroProcessorFilterSpyAttribute | MicroProcessorFilterSpyAttribute | MessageHandlerSpy<Object>", pipelineB.ToString());
        }

        private static MessageHandlerFilterPipeline CreateFilterPipeline<TMessage>(IMessageHandler<TMessage> handler, TMessage message, IMicroProcessorFilter filter) =>
            CreateFilterPipeline(new MessageHandlerDecorator<TMessage>(handler, message, CreateMessageHandlerContext(message)), filter);

        private static MessageHandlerFilterPipeline CreateFilterPipeline(MessageHandler handler, IMicroProcessorFilter filter) =>
            new MessageHandlerFilterPipeline(handler, filter);

        private static MessageHandlerContext CreateMessageHandlerContext(object message) =>
            new MessageHandlerContext(new NullServiceProvider(), Thread.CurrentPrincipal, null, message);

        private static IMicroProcessorFilter CreateFilter() =>
            new MicroProcessorFilterSpyAttribute(MicroProcessorFilterStage.ProcessingStage);
    }
}
