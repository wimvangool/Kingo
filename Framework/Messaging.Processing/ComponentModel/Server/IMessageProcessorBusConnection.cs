using System.Threading.Tasks;

namespace ServiceComponents.ComponentModel.Server
{
    internal interface IMessageProcessorBusConnection : IConnection
    {
        Task HandleAsync<TPublished>(IMessageProcessor processor, TPublished message) where TPublished : class, IMessage<TPublished>;
    }
}
