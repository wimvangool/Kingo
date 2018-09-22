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

        public MicroProcessorOperationTypes OperationType =>
            Current.OperationType;

        public IStackTrace StackTrace =>
            Current.StackTrace;

        public IUnitOfWorkController UnitOfWork =>
            Current.UnitOfWork;

        public IEventBus EventBus =>
            Current.EventBus;             

        public CancellationToken Token =>
            Current.Token;        

        public override string ToString() =>
            Current.ToString();
    }
}
