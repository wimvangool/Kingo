
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerFactoryTests
{    
    internal sealed class MessageHandlerB : IExternalMessageHandler<Command>,
                                            IInternalMessageHandler<DomainEvent>
    {
        public void Handle(Command message)
        {
            MessageCommandRecorder.Current.Record(this, message);
        }

        public void Handle(DomainEvent message)
        {
            MessageCommandRecorder.Current.Record(this, message);
        }
    }
}
