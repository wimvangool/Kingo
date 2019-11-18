namespace Kingo.MicroServices.Controllers
{
    internal abstract class MessageHandlerOperationTestStub<TMessage> : MessageHandlerOperationTest<TMessage>
    {                       
        protected override void Then(TMessage message, IMessageHandlerOperationTestResult result, MicroProcessorOperationTestContext context) =>
            result.IsMessageStream();
    }
}
