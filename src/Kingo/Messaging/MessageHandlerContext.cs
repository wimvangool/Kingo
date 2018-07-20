using System.Security.Principal;
using System.Threading;

namespace Kingo.Messaging
{
    internal sealed class MessageHandlerContext : MicroProcessorContext
    {
        private readonly UnitOfWorkController _controller;
        private EventStream _outputStream;        

        public MessageHandlerContext(IPrincipal principal, CancellationToken? token = null) :
            base(principal, token, new StackTrace())
        {            
            _controller = new UnitOfWorkController();
            _outputStream = new EventStreamImplementation();            
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            UnitOfWorkCore.Dispose();

            base.DisposeManagedResources();
        }

        public override IUnitOfWorkController UnitOfWork =>
            UnitOfWorkCore;

        internal UnitOfWorkController UnitOfWorkCore =>
            _controller;

        internal override EventStream OutputStreamCore =>
            _outputStream;             

        internal void Reset() =>
            _outputStream = new EventStreamImplementation();
    }
}
