using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Kingo.MicroServices.Controllers
{
    internal sealed class MicroServiceBusEndpointStub<TMessage> : IMicroServiceBusEndpoint
    {
        public MicroServiceBusEndpointStub(string serviceName, IEndpointMessageHandler<TMessage> handler)
        {
            Name = serviceName;
            MessageHandlerType = handler.GetType();
            MethodInfo = MessageHandlerType.GetInterfaceMap(typeof(IEndpointMessageHandler<TMessage>)).TargetMethods[0];
            MessageParameterInfo = MethodInfo.GetParameters()[0];
        }

        public string Name
        {
            get;
        }

        public MessageKind MessageKind =>
            MessageKind.Undefined;

        public Type MessageHandlerType
        {
            get;
        }

        public MethodInfo MethodInfo
        {
            get;
        }

        public ParameterInfo MessageParameterInfo
        {
            get;
        }

        public ParameterInfo ContextParameterInfo =>
            null;

        public Task<IMessageHandlerOperationResult> InvokeAsync(IMessage message, CancellationToken? token = null) =>
            throw new NotSupportedException();
    }
}
