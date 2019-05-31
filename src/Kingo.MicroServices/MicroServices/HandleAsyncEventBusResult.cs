namespace Kingo.MicroServices
{
    internal sealed class HandleAsyncEventBusResult : HandleAsyncResult
    {
        private readonly IEventBus _eventBus;

        public HandleAsyncEventBusResult(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override MessageStream Events =>
            _eventBus.ToStream();

        public override int MessageHandlerCount =>
            1;
    }
}
