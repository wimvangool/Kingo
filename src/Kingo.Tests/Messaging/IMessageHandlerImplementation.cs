using System;
using System.Threading.Tasks;

namespace Kingo.Messaging
{
    internal interface IMessageHandlerImplementation
    {
        Task HandleAsync(object message, IMicroProcessorContext context, Type messageHandlerType);
    }
}
