using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class NullQuery : IQuery<object>, IQuery<object, object>
    {
        public Task<object> ExecuteAsync(QueryOperationContext context) =>
            Task.FromResult(new object());

        public Task<object> ExecuteAsync(object message, QueryOperationContext context) =>
            Task.FromResult(message);
    }
}
