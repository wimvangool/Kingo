using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.Messaging
{
    internal interface IMessageProcessorBusConnection : IConnection
    {
        Task HandleAsync<TPublished>(IMessageProcessor processor, TPublished message) where TPublished : class, IMessage<TPublished>;
    }
}
