using Kingo.Reflection;

namespace Kingo.MicroServices
{
    internal abstract class WhenCommandOrEventState<TMessage> : MicroProcessorTestState
    {
        private readonly MicroProcessorTest _test;
        private readonly MicroProcessorTestOperationQueue _givenOperations;

        protected WhenCommandOrEventState(MicroProcessorTest test, MicroProcessorTestOperationQueue givenOperations)
        {
            _test = test;
            _givenOperations = givenOperations;
        }

        protected override MicroProcessorTest Test =>
            _test;

        protected virtual string ToString(string messageHandler) =>
            $"Configuring a {messageHandler} of type '{typeof(IMessageHandler<TMessage>).FriendlyName()}'...";

        protected IReadyToRunMessageHandlerTestState<TMessage> MoveToReadyToRunMessageHandlerTestState(MessageHandlerTestOperation<TMessage> whenOperation) =>
            Test.MoveToState(this, new ReadyToRunMessageHandlerTestState<TMessage>(_test, _givenOperations, whenOperation));
    }
}
