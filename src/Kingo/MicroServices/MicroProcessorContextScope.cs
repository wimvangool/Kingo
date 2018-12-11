using Kingo.Threading;

namespace Kingo.MicroServices
{
    internal sealed class MicroProcessorContextScope : Disposable
    {
        private readonly ContextScope<MicroProcessorContext> _scope;

        public MicroProcessorContextScope(ContextScope<MicroProcessorContext> scope)
        {
            _scope = scope;
        }

        public MicroProcessorContext Context =>
            _scope.Value;

        protected override void DisposeManagedResources() =>
            _scope.Dispose();
    }
}
