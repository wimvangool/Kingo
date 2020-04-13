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
            Test.MoveToState(this, new GivenState(_test, _timeline, _givenOperations));

        public override IWhenCommandOrEventState<TMessage> WhenCommandOrEvent<TMessage>() =>
            Test.MoveToState(this, new WhenCommandOrEventState<TMessage>(_test, _givenOperations));

        public override IWhenRequestState WhenRequest() =>
            Test.MoveToState(this, new WhenRequestState(_test, _givenOperations));

        public override IWhenRequestState<TRequest> WhenRequest<TRequest>() =>
            Test.MoveToState(this, new WhenRequestState<TRequest>(_test, _givenOperations));
    }
}
