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
            public override string ToString() =>
                string.Empty;

            public override void Publish<TEvent>(TEvent message)
            {
                throw NewPublishNotAllowedException();
            }

            private static Exception NewPublishNotAllowedException() =>
                new InvalidOperationException(ExceptionMessages.MessageHandlerContext_NullOutputStream_PublishNotAllowed);
        }

        #endregion

        #region [====== HandleAsyncResultImplementation ======]

        private sealed class HandleAsyncResultImplementation : HandleAsyncResult
        {
            private readonly MessageHandlerContext _context;

            public HandleAsyncResultImplementation(bool isMetadataResult, IMessageStream outputStream, IMessageStream metadataStream, MessageHandlerContext context) :
                base(isMetadataResult, outputStream, metadataStream)
            {
                _context = context;
            }

            internal override HandleAsyncResult RemoveOutputStream() =>
                new HandleAsyncResultImplementation(IsMetadataResult, MessageStream.Empty, MetadataStream, _context);

            internal override void Commit()
            {
                _context._outputStream = CreateOutputStream(IsMetadataResult);
                _context._metadataStream = new EventStreamImplementation();
            }
        }

        #endregion

        private readonly bool _isMetadataContext;
        private EventStream _outputStream;
        private EventStream _metadataStream;

        public MessageHandlerContext(CancellationToken? token = null, MessageStackTrace stackTrace = null) :
            base(token, stackTrace)
        {
            _isMetadataContext = stackTrace != null;
            _outputStream = CreateOutputStream(_isMetadataContext);
            _metadataStream = new EventStreamImplementation();
        }

        public override bool IsMetadataContext =>
            _isMetadataContext;

        public override IEventStream OutputStream =>
            _outputStream;

        public override IEventStream MetadataStream =>
            _metadataStream;

        public HandleAsyncResult CreateHandleAsyncResult() =>
            new HandleAsyncResultImplementation(_isMetadataContext, _outputStream, _metadataStream, this);        

        private static EventStream CreateOutputStream(bool isMetadataContext)
        {
            if (isMetadataContext)
            {
                return new NullOutputStream();
            }
            return new EventStreamImplementation();
        }
    }
}
