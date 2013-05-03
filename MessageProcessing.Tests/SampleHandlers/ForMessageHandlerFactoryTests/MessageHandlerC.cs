
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerFactoryTests
{
    internal sealed class MessageHandlerC : IInternalMessageHandler<DomainEvent>,
                                            IInternalMessageHandler<object>
    {
        public void Handle(DomainEvent message)
        {
            MessageCommandRecorder.Current.Record(this, message);
        }

        public void Handle(object message)
        {
            MessageCommandRecorder.Current.Record(this, message);
        }
    }
}
