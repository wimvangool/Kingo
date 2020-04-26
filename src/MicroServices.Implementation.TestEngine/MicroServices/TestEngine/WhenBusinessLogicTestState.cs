namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenBusinessLogicTestState : MicroProcessorTestState, IWhenBusinessLogicTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenBusinessLogicTestState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public IWhenCommandState<TCommand> Command<TCommand>() =>
            _test.MoveToState(this, new WhenCommandState<TCommand>(_test, _givenOperations));

        public IWhenEventState<TEvent> Event<TEvent>() =>
            _test.MoveToState(this, new WhenEventState<TEvent>(_test, _givenOperations));
    }
}
