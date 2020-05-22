using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal sealed class WhenRequestState : MicroProcessorTestState, IWhenRequestState
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        public WhenRequestState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        public override string ToString() =>
            $"Configuring a query of type '{typeof(IQuery<>).FriendlyName()}'...";

        public IWhenResponseState<TResponse> Returning<TResponse>() =>
            _test.MoveToState(this, new WhenResponseState<TResponse>(_test, _givenOperations));
    }
}
