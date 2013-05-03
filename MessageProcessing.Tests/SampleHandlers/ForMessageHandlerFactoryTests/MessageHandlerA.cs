
namespace YellowFlare.MessageProcessing.SampleHandlers.ForMessageHandlerFactoryTests
{
    internal sealed class MessageHandlerA : IExternalMessageHandler<Command>
    {
        public void Handle(Command message)
        {
            MessageCommandRecorder.Current.Record(this, message);
        }
    }
}
