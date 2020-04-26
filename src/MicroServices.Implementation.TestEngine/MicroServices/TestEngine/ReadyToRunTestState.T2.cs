using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal abstract class ReadyToRunTestState<TOperation, TOutputState> : MicroProcessorTestState
        where TOperation : MicroProcessorTestOperation
        where TOutputState : MicroProcessorTestState
    {
        protected abstract TOperation WhenOperation
        {
            get;
        }

        protected async Task<TOutputState> RunTestAsync() =>
            await Test.MoveToState(this, CreateRunningTestState()).RunAsync(WhenOperation);

        protected abstract RunningTestState<TOperation, TOutputState> CreateRunningTestState();
    }
}
