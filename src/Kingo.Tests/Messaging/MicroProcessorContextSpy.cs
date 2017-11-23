using System;
using System.Threading;

namespace Kingo.Messaging
{
    public sealed class MicroProcessorContextSpy : IMicroProcessorContext
    {
        public IMessageStackTrace Messages =>
             throw new NotImplementedException();

        public IUnitOfWorkController UnitOfWork =>
             throw new NotImplementedException();

        public IEventStream OutputStream =>
             throw new NotImplementedException();

        public IEventStream MetadataStream =>
             throw new NotImplementedException();

        public CancellationToken Token =>
             throw new NotImplementedException();
    }
}
