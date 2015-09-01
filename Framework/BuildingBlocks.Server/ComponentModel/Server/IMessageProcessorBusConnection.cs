using System.Threading.Tasks;

namespace Kingo.BuildingBlocks.ComponentModel.Server
{
    internal interface IMessageProcessorBusConnection : IConnection
    {
        Task HandleAsync<TPublished>(IMessageProcessor processor, TPublished message) where TPublished : class, IMessage<TPublished>;
    }
}
