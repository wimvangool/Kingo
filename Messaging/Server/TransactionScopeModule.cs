using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// Represents a module that creates a <see cref="TransactionScope" /> before calling the next handler.
    /// </summary>    
    /// <remarks>
    /// <para>
    /// This module will inspect each message for declarations of the <see cref="TransactionScopeAttribute" />. If
    /// specified, it will use this attribute to create a new <see cref="TransactionScope" />. Otherwise,
    /// it will use the settings that were passed to one of it's constructors to create a new <see cref="TransactionScope" />.
    /// </para>
    /// <para>
    /// Also note that, in contrast with the default <see cref="IsolationLevel" /> of <see cref="IsolationLevel.Serializable" />
    /// used by the <see cref="TransactionScope" /> class, this module will apply a level of <see cref="IsolationLevel.ReadCommitted" />
    /// when creating a new <see cref="Transaction" /> and the <see cref="IsolationLevel" /> was not explicitly specified.
    /// </para>
    /// </remarks>
    public class TransactionScopeModule : MessageHandlerModule<TransactionScopeAttribute>       
    {        
        private readonly TransactionScopeFactory _transactionScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeModule" /> class.
        /// </summary>        
        public TransactionScopeModule()
            : this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeModule" /> class.
        /// </summary>        
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>                
        public TransactionScopeModule(TransactionScopeOption scopeOption)
            : this(scopeOption, TransactionManager.DefaultTimeout) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeModule" /> class.
        /// </summary>        
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The default timeout to use.</param>
        /// <param name="isolationLevel">
        /// The default <see cref="IsolationLevel" /> to use. If <c>null</c> is specified, the actual <see cref="IsolationLevel" /> applied
        /// will be that of any running <see cref="Transaction" /> or of the .NET Framework's default <see cref="IsolationLevel" />
        /// setting, <see cref="IsolationLevel.Serializable"/>.
        /// </param>        
        public TransactionScopeModule(TransactionScopeOption scopeOption, TimeSpan timeout, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {            
            _transactionScopeFactory = new TransactionScopeFactory(scopeOption, timeout, isolationLevel, EnterpriseServicesInteropOption.None);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeModule" /> class.
        /// </summary>        
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="transactionOptions">The default <see cref="TransactionOptions" /> to use.</param>
        /// <param name="interopOption">The default <see cref="EnterpriseServicesInteropOption" /> to use.</param>        
        public TransactionScopeModule(TransactionScopeOption scopeOption, TransactionOptions transactionOptions, EnterpriseServicesInteropOption interopOption = EnterpriseServicesInteropOption.None)
        {            
            _transactionScopeFactory = new TransactionScopeFactory(scopeOption, transactionOptions.Timeout, transactionOptions.IsolationLevel, interopOption);
        }        

        /// <summary>
        /// Wraps the invocation of the next <see cre="Handler" /> in a newl created <see cref="TransactionScope" />.
        /// </summary>
        /// <param name="handler">The handler to invoke.</param>
        /// <param name="attributes">All attributes of type <see cref="TransactionScopeAttribute" />. declared on the message.</param>        
        protected override void InvokeHandler(IMessageHandler handler, IEnumerable<TransactionScopeAttribute> attributes)
        {            
            using (var scope = CreateTransactionScope(attributes.SingleOrDefault()))
            {
                handler.Invoke();

                scope.Complete();
            }
        }

        private TransactionScope CreateTransactionScope(TransactionScopeAttribute attribute)
        {
            return attribute == null
                ? _transactionScopeFactory.CreateTransactionScope()
                : attribute.CreateTransactionScope();
        }
    }
}
