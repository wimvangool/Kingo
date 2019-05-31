namespace Kingo.MicroServices
{
    internal sealed class HandleAsyncMessageStreamResult : HandleAsyncResult
    {        
        public HandleAsyncMessageStreamResult(MessageStream events, int messageHandlerCount)
        {
            Events = events;
            MessageHandlerCount = messageHandlerCount;
        }

        public override MessageStream Events
        {
            get;
        }

        public override int MessageHandlerCount
        {
            get;
        }
    }
}
