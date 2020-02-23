using System.Threading.Tasks;

namespace Kingo.MicroServices.TestEngine
{
    internal sealed class NotReadyState : MicroProcessorTestState
    {
        public NotReadyState(MicroProcessorTest test)
        {
            Test = test;
        }

        protected override MicroProcessorTest Test
        {
            get;
        }

        public override string ToString() =>
            $"Not ready - waiting for {nameof(SetupAsync)}...";

        public override Task SetupAsync()
        {
            Test.MoveToState(this, new ReadyToConfigureTestState(Test));
            return Task.CompletedTask;
        }

        public override Task TearDownAsync()
        {
            Test.MoveToState(this, new NotReadyState(Test));
            return Task.CompletedTask;
        }
    }
}
