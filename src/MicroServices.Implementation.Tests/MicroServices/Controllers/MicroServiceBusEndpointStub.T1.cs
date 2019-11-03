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
            ServiceName = serviceName;
            MessageHandlerType = handler.GetType();
            MethodInfo = MessageHandlerType.GetInterfaceMap(typeof(IEndpointMessageHandler<TMessage>)).TargetMethods[0];
            MessageParameterInfo = MethodInfo.GetParameters()[0];
        }

        public string ServiceName
        {
            get;
        }

        public MessageKind MessageKind =>
            MessageKind.Unspecified;

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

        public Task<IMessageHandlerOperationResult> InvokeAsync(IMessageEnvelope message, CancellationToken? token = null) =>
            throw new NotSupportedException();
    }
}
