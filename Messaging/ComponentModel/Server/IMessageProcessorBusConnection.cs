namespace System.ComponentModel.Server
{
    internal interface IMessageProcessorBusConnection : IConnection
    {
        void Handle<TPublished>(IMessageProcessor processor, TPublished message) where TPublished : class, IMessage<TPublished>;
    }
}
