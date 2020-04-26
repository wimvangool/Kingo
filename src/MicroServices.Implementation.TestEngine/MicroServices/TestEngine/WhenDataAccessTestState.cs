namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenDataAccessTestState : MicroProcessorTestState, IWhenDataAccessTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenDataAccessTestState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public IWhenRequestState Request() =>
            _test.MoveToState(this, new WhenRequestState(_test, _givenOperations));

        public IWhenRequestState<TRequest> Request<TRequest>() =>
            _test.MoveToState(this, new WhenRequestState<TRequest>(_test, _givenOperations));
    }
}
