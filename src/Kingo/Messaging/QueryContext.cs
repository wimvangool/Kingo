using System;
using System.Security.Principal;
using System.Threading;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class QueryContext : MicroProcessorContext
    {
        #region [====== NullOutputStream ======]       

        private sealed class NullOutputStream : NullEventStream
        {           
            public override void Publish<TEvent>(TEvent message) =>
                throw NewPublishNotAllowedException();

            private static Exception NewPublishNotAllowedException() =>
                new InvalidOperationException(ExceptionMessages.QueryContext_NullOutputStream_PublishNotAllowed);
        }

        #endregion                

        public QueryContext(IPrincipal principal, CancellationToken? token = null) :
            base(principal, token, new StackTrace())
        {           
            OutputStreamCore = new NullOutputStream();            
        }

        public override IUnitOfWorkController UnitOfWork =>
            UnitOfWorkController.None;

        internal override EventStream OutputStreamCore
        {
            get;
        }       
    }
}
