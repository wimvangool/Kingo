using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Kingo.Resources;
using Kingo.Threading;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the context in which a certain message is being handled.
    /// </summary>    
    public abstract class MicroProcessorContext : Disposable, IMicroProcessorContext
    {
        #region [====== NullContext ======]

        private sealed class NullContext : IMicroProcessorContext
        {
            private readonly IEventStream _nullStream = new NullStream();

            public NullContext()
            {
                StackTrace = new EmpyStackTrace();                                               
            }

            public IPrincipal Principal =>
                Thread.CurrentPrincipal;

            public IClaimsProvider ClaimsProvider =>
                new ClaimsProvider(Principal);

            public IMessageStackTrace StackTrace
            {
                get;
            }

            public IUnitOfWorkController UnitOfWork =>
                UnitOfWorkController.None;

            public IEventStream OutputStream =>
                _nullStream;

            public IEventStream MetadataStream =>
                _nullStream;

            public CancellationToken Token =>
                CancellationToken.None;                        

            public override string ToString() =>
                string.Empty;
        }        

        private sealed class EmpyStackTrace : EmptyList<MessageInfo>, IMessageStackTrace
        {
            MessageInfo IMessageStackTrace.Current =>
                null;

            public override string ToString() =>
                string.Empty;
        }

        private sealed class NullStream : ReadOnlyList<object>, IEventStream
        {
            public override int Count =>
                0;

            public override IEnumerator<object> GetEnumerator() =>
                Enumerable.Empty<object>().GetEnumerator();

            public void Publish<TEvent>(TEvent message)
            {                
                throw NewPublishNotSupportedException(typeof(TEvent));
            }            

            private static Exception NewPublishNotSupportedException(Type messageType)
            {
                var messageFormat = ExceptionMessages.NullStream_PublishNotSupported;
                var message = string.Format(messageFormat, messageType.FriendlyName());
                return new InvalidOperationException(message);
            }            
        }

        #endregion

        #region [====== IMicroProcessorContext ======]
        
        private readonly MessageStackTrace _stackTrace;        
        private readonly UnitOfWorkController _unitOfWorkController;        
        private readonly CancellationToken _token;                

        internal MicroProcessorContext(IPrincipal principal, CancellationToken? token, MessageStackTrace stackTrace = null)
        {
            Principal = principal;
            ClaimsProvider = new ClaimsProvider(principal);

            _stackTrace = stackTrace ?? new MessageStackTrace();
            _unitOfWorkController = new UnitOfWorkController();            
            _token = token ?? CancellationToken.None;
        }

        /// <inheritdoc />
        public IPrincipal Principal
        {
            get;
        }

        /// <inheritdoc />
        public IClaimsProvider ClaimsProvider
        {
            get;
        }

        internal MessageHandlerContext CreateMetadataContext() =>
            new MessageHandlerContext(Principal, _token, _stackTrace.Copy());

        IMessageStackTrace IMicroProcessorContext.StackTrace =>
            _stackTrace;

        internal MessageStackTrace Messages =>
            _stackTrace;

        IUnitOfWorkController IMicroProcessorContext.UnitOfWork =>
            _unitOfWorkController;

        internal UnitOfWorkController UnitOfWork =>
            _unitOfWorkController;        

        /// <inheritdoc />
        public abstract IEventStream OutputStream
        {
            get;
        }

        /// <inheritdoc />
        public abstract IEventStream MetadataStream
        {
            get;
        }        

        /// <inheritdoc />
        public CancellationToken Token =>
            _token;        

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {            
            _unitOfWorkController.Dispose();

            base.DisposeManagedResources();
        }

        /// <inheritdoc />
        public override string ToString() =>
            _stackTrace.ToString();        

        #endregion               

        #region [====== Current ======]

        private static readonly Context<MicroProcessorContext> _Context = new Context<MicroProcessorContext>();

        /// <summary>
        /// Represents a null-context.
        /// </summary>
        public static readonly IMicroProcessorContext None = new NullContext();        

        /// <summary>
        /// Represents the current context.
        /// </summary>
        public static readonly IMicroProcessorContext Current = new MicroProcessorContextRelay(_Context);       

        internal static MicroProcessorContextScope CreateContextScope(MicroProcessorContext context) =>
            new MicroProcessorContextScope(_Context.OverrideAsyncLocal(context));

        #endregion
    }
}
