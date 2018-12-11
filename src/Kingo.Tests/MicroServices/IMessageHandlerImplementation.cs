using System;
using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal interface IMessageHandlerImplementation
    {
        Task HandleAsync(object message, MessageHandlerContext context, Type messageHandlerType);
    }
}
