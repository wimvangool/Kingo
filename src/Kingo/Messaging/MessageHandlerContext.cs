using System;
using System.Threading;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerContext : MicroProcessorContext
    {
        #region [====== NullOutputStream ======]       

        private sealed class NullOutputStream : NullEventStream
        {
            public override void Publish<TEvent>(TEvent message)
            {
                throw NewPublishNotAllowedException();
            }

            private static Exception NewPublishNotAllowedException() =>
                new InvalidOperationException(ExceptionMessages.MessageHandlerContext_NullOutputStream_PublishNotAllowed);
        }

        #endregion

        private readonly bool _isMetadataController;
        private EventStream _outputStream;
        private EventStream _metadataStream;

        public MessageHandlerContext(CancellationToken? token = null, MessageStackTrace stackTrace = null) :
            base(token, stackTrace)
        {
            _isMetadataController = stackTrace != null;
            _outputStream = CreateOutputStream(_isMetadataController);
            _metadataStream = new EventStreamImplementation();
        }

        public override IEventStream OutputStream =>
            _outputStream;

        public override IEventStream MetadataStream =>
            _metadataStream;

        public HandleAsyncResult CreateHandleAsyncResult() =>
            new HandleAsyncResult(_isMetadataController, _outputStream, _metadataStream);

        internal override void Reset()
        {
            _outputStream = CreateOutputStream(_isMetadataController);
            _metadataStream = new EventStreamImplementation();
        }

        private static EventStream CreateOutputStream(bool isMetadataController)
        {
            if (isMetadataController)
            {
                return new NullOutputStream();
            }
            return new EventStreamImplementation();
        }
    }
}
