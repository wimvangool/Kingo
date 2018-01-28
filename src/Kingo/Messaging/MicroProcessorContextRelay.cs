using System.Security.Principal;
using System.Threading;
using Kingo.Threading;

namespace Kingo.Messaging
{
    internal sealed class MicroProcessorContextRelay : IMicroProcessorContext
    {
        private readonly Context<MicroProcessorContext> _context;

        public MicroProcessorContextRelay(Context<MicroProcessorContext> context)
        {
            _context = context;
        }

        private IMicroProcessorContext Current =>
            _context.Current ?? MicroProcessorContext.None;

        public IPrincipal Principal =>
            Current.Principal;

        public IClaimsProvider ClaimsProvider =>
            Current.ClaimsProvider;

        public IMessageStackTrace Messages =>
            Current.Messages;

        public IUnitOfWorkController UnitOfWork =>
            Current.UnitOfWork;

        public IEventStream OutputStream =>
            Current.OutputStream;

        public IEventStream MetadataStream =>
            Current.MetadataStream;        

        public CancellationToken Token =>
            Current.Token;        

        public override string ToString() =>
            Current.ToString();
    }
}
