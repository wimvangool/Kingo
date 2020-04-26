namespace Kingo.MicroServices.TestEngine
{
    internal sealed class ReadyToConfigureTestState : MicroProcessorTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestTimeline _timeline;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public ReadyToConfigureTestState(MicroProcessorTest test) :
            this(test, new MicroProcessorTestTimeline(), MicroProcessorTestOperationQueue.Empty) { }

        public ReadyToConfigureTestState(MicroProcessorTest test, MicroProcessorTestTimeline timeline, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _timeline = timeline;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Ready to configure test ({_givenOperations}) | {_timeline}";

        public override IGivenState Given() =>
            _test.MoveToState(this, new GivenState(_test, _timeline, _givenOperations));

        public override IWhenBusinessLogicTestState WhenBusinessLogicTest() =>
            _test.MoveToState(this, new WhenBusinessLogicTestState(_test, _givenOperations));

        public override IWhenDataAccessTestState WhenDataAccessTest() =>
            _test.MoveToState(this, new WhenDataAccessTestState(_test, _givenOperations));
    }
}
