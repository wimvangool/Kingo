using System;
using System.Security.Principal;
using System.Threading;
using Kingo.Threading;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the context in which a <see cref="MicroProcessor"/> operates.
    /// </summary>    
    public abstract class MicroProcessorContext
    {
        private readonly Lazy<ClaimsProvider> _claimsProvider;

        internal MicroProcessorContext()
        {            
            _claimsProvider = new Lazy<ClaimsProvider>(() => new ClaimsProvider(Principal));
        }

        /// <summary>
        /// Returns the service provider associated with this context.
        /// </summary>
        public abstract IServiceProvider ServiceProvider
        {
            get;
        }                

        #region [====== SecurityContext ======]

        /// <summary>
        /// Represents a <see cref="IClaimsProvider"/> that provides access to the claims of the associated <see cref="Principal"/>.
        /// </summary>
        public IClaimsProvider ClaimsProvider =>
            _claimsProvider.Value;

        /// <summary>
        /// The principal that is associated to the current operation.
        /// </summary>
        public abstract IPrincipal Principal
        {
            get;
        }        

        #endregion

        #region [====== OperationContext ======]

        /// <summary>
        /// Represents the operation that is currently being executed.
        /// </summary>
        public abstract MicroProcessorOperation Operation
        {
            get;
        }       

        /// <summary>
        /// The token that signals if the current operation should be cancelled.
        /// </summary>
        public abstract CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            string.Join(" -> ", Operation.StackTrace());

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Current ======]

        private static readonly Context<MicroProcessorContext> _Context = new Context<MicroProcessorContext>();

        /// <summary>
        /// Returns the current context, or <c>null</c> if there isn't any.
        /// </summary>
        public static MicroProcessorContext Current =>
            _Context.Current;

        internal static IDisposable CreateScope(MicroProcessorContext context) =>
            DependencyContext.CreateScope(_Context.OverrideAsyncLocal(context));

        #endregion        
    }
}
