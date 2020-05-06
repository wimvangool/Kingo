using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal static class MicroServiceBusEndpointExtensions
    {
        public static Task<IMessageHandlerOperationResult> InvokeAsync(this IMicroServiceBusEndpoint endpoint, object message, CancellationToken? token = null) =>
            endpoint.InvokeAsync(CreateMessage(message, endpoint.MessageKind), token);

        private static IMessage CreateMessage(object content, MessageKind messageKind) =>
            BuildMessageFactory(content, messageKind).CreateMessage(content);

        private static IMessageFactory BuildMessageFactory(object content, MessageKind messageKind)
        {
            var messageFactoryBuilder = new MessageFactoryBuilder();
            messageFactoryBuilder.Add(content.GetType(), messageKind);
            return messageFactoryBuilder.BuildMessageFactory();
        }
    }
}
