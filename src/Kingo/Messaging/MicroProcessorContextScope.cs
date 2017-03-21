using System.Threading.Tasks;
using Kingo.Threading;

namespace Kingo.Messaging
{
    internal sealed class MicroProcessorContextScope : Disposable
    {
        private readonly ContextScope<MicroProcessorContext> _scope;

        public MicroProcessorContextScope(ContextScope<MicroProcessorContext> scope)
        {
            _scope = scope;
        }             

        public Task CompleteAsync() =>
            _scope.Value.UnitOfWork.CompleteAsync();

        protected override void DisposeManagedResources()
        {
            _scope.Value.Dispose();
            _scope.Dispose();
        }
    }
}
