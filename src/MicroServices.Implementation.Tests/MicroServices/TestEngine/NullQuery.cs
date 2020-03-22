using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class NullQuery : IQuery<object>, IQuery<object, object>
    {
        public Task<object> ExecuteAsync(IQueryOperationContext context) =>
            Task.FromResult(new object());

        public Task<object> ExecuteAsync(object message, IQueryOperationContext context) =>
            Task.FromResult(message);
    }
}
