using System;
using System.Threading;

namespace Kingo.Messaging
{
    public sealed class MicroProcessorContextSpy : IMicroProcessorContext
    {
        public IMessageStackTrace Messages
        {
            get { throw new NotImplementedException(); }
        }

        public IUnitOfWorkController UnitOfWork
        {
            get { throw new NotImplementedException(); }
        }

        public IEventStream OutputStream
        {
            get { throw new NotImplementedException(); }
        }

        public IEventStream MetadataStream
        {
            get { throw new NotImplementedException(); }
        }

        public CancellationToken Token
        {
            get { throw new NotImplementedException(); }
        }
    }
}
