using System;
using System.Security.Principal;
using System.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> executes a query.
    /// </summary>
    public sealed class QueryContext : MicroProcessorContext
    {        
        internal QueryContext(IServiceProvider serviceProvider, IPrincipal principal, CancellationToken? token, object messageIn = null)
        {
            ServiceProvider = serviceProvider;
            Principal = principal;
            Token = token ?? CancellationToken.None;
            Operation = new MicroProcessorOperation(MicroProcessorOperationTypes.Query, messageIn);
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
        /// Returns the current query-context, or <c>null</c> if there isn't any.
        /// </summary>
        public new static QueryContext Current =>
            MicroProcessorContext.Current as QueryContext;                        
    }
}
