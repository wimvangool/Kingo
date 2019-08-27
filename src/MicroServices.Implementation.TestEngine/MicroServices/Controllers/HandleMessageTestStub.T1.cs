namespace Kingo.MicroServices.Controllers
{
    internal abstract class HandleMessageTestStub<TMessage> : HandleMessageTest<TMessage>
    {                       
        protected override void Then(TMessage message, IHandleMessageResult result, MicroProcessorOperationTestContext context) =>
            result.IsEventStream();
    }
}
