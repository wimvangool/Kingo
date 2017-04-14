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

        #region [====== ExecuteAsyncResultImplementation ======]

        private sealed class ExecuteAsyncResultImplementation<TMessageOut> : ExecuteAsyncResult<TMessageOut>
        {
            private readonly QueryContext _context; 

            public ExecuteAsyncResultImplementation(TMessageOut message, IMessageStream metadataStream, QueryContext context) :
                base(message, metadataStream)
            {
                _context = context;
            }

            internal override void Commit()
            {
                _context._outputStream = new NullOutputStream();
                _context._metadataStream = new EventStreamImplementation();
            }
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
            new ExecuteAsyncResultImplementation<TMessageOut>(message, _metadataStream, this);               
    }
}
