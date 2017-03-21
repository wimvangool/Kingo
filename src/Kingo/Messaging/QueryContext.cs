using System;
using System.Threading;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class QueryContext : MicroProcessorContext
    {
        #region [====== NullOutputStream ======]       

        private sealed class NullOutputStream : NullEventStream
        {           
            public override void Publish<TEvent>(TEvent message)
            {
                throw NewPublishNotAllowedException();
            }

            private static Exception NewPublishNotAllowedException() =>
                new InvalidOperationException(ExceptionMessages.QueryContext_NullOutputStream_PublishNotAllowed);
        }

        #endregion

        private EventStream _outputStream;
        private EventStream _metadataStream;

        public QueryContext(CancellationToken? token = null) :
            base(token)
        {           
            _outputStream = new NullOutputStream();
            _metadataStream = new EventStreamImplementation();
        }

        public override IEventStream OutputStream =>
            _outputStream;

        public override IEventStream MetadataStream =>
            _metadataStream;       

        public ExecuteAsyncResult<TMessageOut> CreateExecuteAsyncResult<TMessageOut>(TMessageOut message) =>
            new ExecuteAsyncResult<TMessageOut>(message, _metadataStream);

        internal override void Reset()
        {
            _outputStream = new NullOutputStream();
            _metadataStream = new EventStreamImplementation();
        }        
    }
}
