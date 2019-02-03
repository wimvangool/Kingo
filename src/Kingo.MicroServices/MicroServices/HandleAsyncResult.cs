namespace Kingo.MicroServices
{    
    internal sealed class HandleAsyncResult : InvokeAsyncResult<MessageStream>
    {
        private readonly IEventBus _eventBus;

        public HandleAsyncResult(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public override MessageStream GetValue() =>
            _eventBus.ToStream();

        public override string ToString() =>
            _eventBus.ToString();
    }
}
