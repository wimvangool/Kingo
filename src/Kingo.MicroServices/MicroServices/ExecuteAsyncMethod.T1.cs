using System.Reflection;
using System.Threading.Tasks;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class ExecuteAsyncMethod<TResponse> : ExecuteAsyncMethod, IQuery<TResponse>
    {
        private readonly IQuery<TResponse> _query;
        private readonly IParameterAttributeProvider _contextParameter;

        public ExecuteAsyncMethod(IQuery<TResponse> query) :
            this(query, QueryType.FromInstance(query), QueryInterface.FromType<TResponse>()) { }

        private ExecuteAsyncMethod(IQuery<TResponse> query, Query component, QueryInterface @interface) :
            this(query, component, @interface.CreateMethodAttributeProvider(component)) { }

        private ExecuteAsyncMethod(IQuery<TResponse> query, Query component, MethodAttributeProvider attributeProvider) :
            this(query, component, attributeProvider, attributeProvider.Info.GetParameters()) { }

        private ExecuteAsyncMethod(IQuery<TResponse> query, Query component, MethodAttributeProvider attributeProvider, ParameterInfo[] parameters) :
            base(component, attributeProvider)
        {
            _query = query;
            _contextParameter = new ParameterAttributeProvider(parameters[0]);
        }

        public override IParameterAttributeProvider MessageParameter =>
            null;

        public override IParameterAttributeProvider ContextParameter =>
            _contextParameter;

        public Task<TResponse> ExecuteAsync(QueryOperationContext context) =>
            _query.ExecuteAsync(context);

        public override string ToString() =>
            $"{Query.Type.FriendlyName()}.{nameof(IQuery<object>.ExecuteAsync)}(...)";
    }
}
