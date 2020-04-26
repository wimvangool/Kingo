namespace Kingo.MicroServices.TestEngine
{
    internal sealed class WhenDataAccessTestState : WhenBusinessLogicTestState, IWhenDataAccessTestState
    {
        public WhenDataAccessTestState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations) :
            base(test, givenOperations) { }

        public IWhenRequestState Request() =>
            Test.MoveToState(this, new WhenRequestState(Test, GivenOperations));

        public IWhenRequestState<TRequest> Request<TRequest>() =>
            Test.MoveToState(this, new WhenRequestState<TRequest>(Test, GivenOperations));
    }
}
