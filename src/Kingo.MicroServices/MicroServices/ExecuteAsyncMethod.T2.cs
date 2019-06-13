using System.Reflection;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteAsyncMethod<TRequest, TResponse> : ExecuteAsyncMethod, IQuery<TRequest, TResponse>
    {
        private readonly IQuery<TRequest, TResponse> _query;
        private readonly IParameterAttributeProvider _messageParameter;
        private readonly IParameterAttributeProvider _contextParameter;

        public ExecuteAsyncMethod(IQuery<TRequest, TResponse> query) :
            this(query, QueryType.FromInstance(query), QueryInterface.FromType<TRequest, TResponse>()) { }

        private ExecuteAsyncMethod(IQuery<TRequest, TResponse> query, Query component, QueryInterface @interface) :
            this(query, component, @interface.CreateMethodAttributeProvider(component)) { }

        private ExecuteAsyncMethod(IQuery<TRequest, TResponse> query, Query component, MethodAttributeProvider attributeProvider) :
            this(query, component, attributeProvider, attributeProvider.Info.GetParameters()) { }

        private ExecuteAsyncMethod(IQuery<TRequest, TResponse> query, Query component, MethodAttributeProvider attributeProvider, ParameterInfo[] parameters) :
            base(component, attributeProvider)
        {
            _query = query;
            _messageParameter = new ParameterAttributeProvider(parameters[0]);
            _contextParameter = new ParameterAttributeProvider(parameters[1]);
        }

        public override IParameterAttributeProvider MessageParameter =>
            _messageParameter;

        public override IParameterAttributeProvider ContextParameter =>
            _contextParameter;

        public Task<TResponse> ExecuteAsync(TRequest message, QueryOperationContext context) =>
            _query.ExecuteAsync(message, context);

        public override string ToString() =>
            $"{Query.Type.FriendlyName()}.{nameof(IQuery<object, object>.ExecuteAsync)}({MessageParameter.Type.FriendlyName()}, ...)";
    }
}
