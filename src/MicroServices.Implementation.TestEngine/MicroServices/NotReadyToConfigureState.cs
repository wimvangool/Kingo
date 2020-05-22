using System.Threading.Tasks;

namespace Kingo.MicroServices
{
    internal sealed class NotReadyToConfigureState : MicroProcessorTestState
    {
        public NotReadyToConfigureState(MicroProcessorTest test)
        {
            Test = test;
        }

        protected override MicroProcessorTest Test
        {
            get;
        }

        public override string ToString() =>
            $"Not ready to configure test - waiting for {nameof(SetupAsync)}...";

        public override Task SetupAsync()
        {
            Test.MoveToState(this, new ReadyToConfigureTestState(Test));
            return Task.CompletedTask;
        }

        public override Task TearDownAsync()
        {
            Test.MoveToState(this, new NotReadyToConfigureState(Test));
            return Task.CompletedTask;
        }
    }
}
