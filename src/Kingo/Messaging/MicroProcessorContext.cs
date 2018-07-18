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
                OutputStream = new NullStream();
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

            public IEventStream OutputStream
            {
                get;
            }

            public CancellationToken Token =>
                CancellationToken.None;                        

            public override string ToString() =>
                string.Empty;
        }        

        private sealed class EmpyStackTrace : EmptyList<MessageInfo>, IMessageStackTrace
        {
            public MessageSources CurrentSource =>
                MessageSources.None;

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

        internal MicroProcessorContext(IPrincipal principal, CancellationToken? token, MessageStackTrace stackTrace)
        {
            Principal = principal;
            ClaimsProvider = new ClaimsProvider(principal);
            StackTraceCore = stackTrace;
            UnitOfWorkCore = new UnitOfWorkController();            
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
        public IMessageStackTrace StackTrace =>
            StackTraceCore;

        internal MessageStackTrace StackTraceCore
        {
            get;
        }

        /// <inheritdoc />
        public IUnitOfWorkController UnitOfWork =>
            UnitOfWorkCore;

        internal UnitOfWorkController UnitOfWorkCore
        {
            get;
        }

        /// <inheritdoc />
        public IEventStream OutputStream =>
            OutputStreamCore;
        
        internal abstract EventStream OutputStreamCore
        {
            get;
        }

        /// <inheritdoc />
        public CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {            
            UnitOfWorkCore.Dispose();

            base.DisposeManagedResources();
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
