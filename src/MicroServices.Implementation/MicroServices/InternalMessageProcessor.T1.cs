namespace Kingo.MicroServices
{
    internal abstract class InternalMessageProcessor<TContext> where TContext : MicroProcessorOperationContext
    {
        protected abstract TContext Context
        {
            get;
        }

        protected Message<TContent> CreateMessage<TContent>(MessageKind messageKind, TContent message) =>
            CreateInternalMessage(messageKind, message).CorrelateWith(Context.StackTrace.CurrentOperation.Message);

        private Message<TContent> CreateInternalMessage<TContent>(MessageKind messageKind, TContent message) =>
            Context.Processor.MessageFactory.CreateMessage(messageKind, MessageDirection.Internal, MessageHeader.Unspecified, message);
    }
}
