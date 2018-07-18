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

        public IMessageStackTrace StackTrace =>
            Current.StackTrace;

        public IUnitOfWorkController UnitOfWork =>
            Current.UnitOfWork;

        public IEventStream OutputStream =>
            Current.OutputStream;             

        public CancellationToken Token =>
            Current.Token;        

        public override string ToString() =>
            Current.ToString();
    }
}
