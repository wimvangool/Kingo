namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ReadyState : MicroProcessorTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestTimeline _timeline;
        private readonly GivenOperationQueue _operations;

        public ReadyState(MicroProcessorTest test) :
            this(test, new MicroProcessorTestTimeline(), GivenOperationQueue.Empty) { }

        public ReadyState(MicroProcessorTest test, MicroProcessorTestTimeline timeline, GivenOperationQueue operations)
        {
            _test = test;
            _timeline = timeline;
            _operations = operations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Ready ({_operations})";

        public override IGivenState Given() =>
            Test.MoveToState(this, new GivenState(_test, _timeline, _operations));
    }
}
