using System.Threading.Tasks;

namespace Syztem.ComponentModel.Server
{
    internal interface IMessageProcessorBusConnection : IConnection
    {
        Task HandleAsync<TPublished>(IMessageProcessor processor, TPublished message) where TPublished : class, IMessage<TPublished>;
    }
}
