using System;
using System.Security.Principal;
using System.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> invokes a <see cref="IMessageHandler{TMessage}"/>.
    /// </summary>
    public sealed class MessageHandlerContext : MicroProcessorContext
    {
        internal MessageHandlerContext(IServiceProvider serviceProvider, IPrincipal principal, CancellationToken? token, object message)           
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            ServiceProvider = serviceProvider;
            Principal = principal;
            Token = token ?? CancellationToken.None;
            Operation = new MicroProcessorOperation(MicroProcessorOperationTypes.InputStream, message);
            UnitOfWork = new UnitOfWork();
            EventBus = new EventBus();
        }

        private MessageHandlerContext(MessageHandlerContext parent, object message)
        {
            ServiceProvider = parent.ServiceProvider;
            Principal = parent.Principal;
            Token = parent.Token;
            Operation = parent.Operation.Push(MicroProcessorOperationTypes.OutputStream, message);
            UnitOfWork = parent.UnitOfWork;
            EventBus = new EventBus();
        }

        /// <inheritdoc />
        public override IServiceProvider ServiceProvider
        {
            get;
        }

        /// <inheritdoc />
        public override IPrincipal Principal
        {
            get;
        }

        /// <inheritdoc />
        public override CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        public override MicroProcessorOperation Operation
        {
            get;
        }

        /// <summary>
        /// Represents the unit of work that is associated to the current operation.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get;
        }

        /// <summary>
        /// Represents the event bus to which all events resulting from the current operation can be published.
        /// </summary>
        public IEventBus EventBus
        {
            get;
        }

        internal MessageHandlerContext CreateContext(object message) =>
            new MessageHandlerContext(this, message);

        /// <summary>
        /// Returns the current message handler-context, or <c>null</c> if there isn't any.
        /// </summary>
        public new static MessageHandlerContext Current =>
            MicroProcessorContext.Current as MessageHandlerContext;        
    }
}
