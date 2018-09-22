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
            public NullContext()
            {
                StackTrace = new EmpyStackTrace();   
                EventBus = new NullStream();
            }

            public IPrincipal Principal =>
                Thread.CurrentPrincipal;

            public IClaimsProvider ClaimsProvider =>
                new ClaimsProvider(Principal);

            public MicroProcessorOperationTypes OperationType =>
                MicroProcessorOperationTypes.None;

            public IStackTrace StackTrace
            {
                get;
            }

            public IUnitOfWorkController UnitOfWork =>
                UnitOfWorkController.None;

            public IEventBus EventBus
            {
                get;
            }

            public CancellationToken Token =>
                CancellationToken.None;                        

            public override string ToString() =>
                string.Empty;
        }        

        private sealed class EmpyStackTrace : EmptyList<MicroProcessorOperation>, IStackTrace
        {
            public MicroProcessorOperationTypes CurrentSource =>
                MicroProcessorOperationTypes.None;

            public bool IsEmpty =>
                true;

            MicroProcessorOperation IStackTrace.CurrentOperation =>
                null;

            public override string ToString() =>
                string.Empty;
        }

        private sealed class NullStream : ReadOnlyList<object>, IEventBus
        {
            public override int Count =>
                0;

            public override IEnumerator<object> GetEnumerator() =>
                Enumerable.Empty<object>().GetEnumerator();

            public void Publish<TEvent>(TEvent message) =>
                throw NewPublishNotSupportedException(typeof(TEvent));

            private static Exception NewPublishNotSupportedException(Type messageType)
            {
                var messageFormat = ExceptionMessages.NullStream_PublishNotSupported;
                var message = string.Format(messageFormat, messageType.FriendlyName());
                return new InvalidOperationException(message);
            }            
        }

        #endregion

        #region [====== IMicroProcessorContext ======]                              

        internal MicroProcessorContext(IPrincipal principal, CancellationToken? token, StackTrace stackTrace)
        {
            Principal = principal;
            ClaimsProvider = new ClaimsProvider(principal);
            StackTraceCore = stackTrace;            
            Token = token ?? CancellationToken.None;
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

        /// <inheritdoc />
        public MicroProcessorOperationTypes OperationType =>
            StackTrace.IsEmpty ? MicroProcessorOperationTypes.None : StackTrace.CurrentOperation.Type;

        /// <inheritdoc />
        public IStackTrace StackTrace =>
            StackTraceCore;

        internal StackTrace StackTraceCore
        {
            get;
        }

        /// <inheritdoc />
        public abstract IUnitOfWorkController UnitOfWork
        {
            get;
        }        

        /// <inheritdoc />
        IEventBus IMicroProcessorContext.EventBus =>
            EventBus;
        
        internal abstract EventBus EventBus
        {
            get;
        }

        /// <inheritdoc />
        public CancellationToken Token
        {
            get;
        }        

        /// <inheritdoc />
        public override string ToString() =>
            StackTraceCore.ToString();        

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

        internal static MicroProcessorContextScope CreateScope(MicroProcessorContext context) =>
            new MicroProcessorContextScope(_Context.OverrideAsyncLocal(context));

        #endregion
    }
}
