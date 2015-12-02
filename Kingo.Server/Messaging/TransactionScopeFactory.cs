using System;
using System.Transactions;

namespace Kingo.Messaging
{
    /// <summary>
    /// Represents a factory for <see cref="TransactionScope">TransactionScopes</see>.
    /// </summary>
    public sealed class TransactionScopeFactory : ITransactionScopeFactory
    {
        private readonly TransactionScopeOption _scopeOption;
        private readonly TransactionOptions _transactionOptions;        

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeFactory" /> class.
        /// </summary>        
        public TransactionScopeFactory()
            : this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeFactory" /> class.
        /// </summary>        
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>                
        public TransactionScopeFactory(TransactionScopeOption scopeOption)
            : this(scopeOption, TransactionManager.DefaultTimeout) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeFactory" /> class.
        /// </summary>        
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The default timeout to use.</param>
        /// <param name="isolationLevel">
        /// The default <see cref="IsolationLevel" /> to use. If <see cref="IsolationLevel.Unspecified" /> is specified,
        /// the actual <see cref="IsolationLevel" /> applied will be that of any running <see cref="Transaction" /> or
        /// of the .NET Framework's default <see cref="IsolationLevel" /> setting, <see cref="IsolationLevel.Serializable"/>.
        /// </param>                
        public TransactionScopeFactory(TransactionScopeOption scopeOption, TimeSpan timeout, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
            : this(scopeOption, new TransactionOptions
            { Timeout = timeout, IsolationLevel = isolationLevel }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeFactory" /> class.
        /// </summary>        
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="transactionOptions">The default <see cref="TransactionOptions" /> to use.</param>        
        public TransactionScopeFactory(TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
        {
            _scopeOption = scopeOption;
            _transactionOptions = transactionOptions;            
        }           

        /// <inheritdoc />
        public TransactionScope CreateTransactionScope()
        {               
            if (_transactionOptions.IsolationLevel == IsolationLevel.Unspecified)
            {
                var currentTransaction = Transaction.Current;
                if (currentTransaction == null || _scopeOption == TransactionScopeOption.RequiresNew)
                {                    
                    return CreateTransactionScope(_scopeOption, ApplyIsolationLevel(_transactionOptions, IsolationLevel.ReadCommitted));
                }
                return CreateTransactionScope(_scopeOption, new TransactionOptions
                {
                    Timeout = _transactionOptions.Timeout,
                    IsolationLevel = currentTransaction.IsolationLevel
                });
            }
            return CreateTransactionScope(_scopeOption, _transactionOptions);       
        }        

        private static TransactionScope CreateTransactionScope(TransactionScopeOption option, TransactionOptions options)
        {
            return new TransactionScope(option, options, TransactionScopeAsyncFlowOption.Enabled);
        }        

        private static TransactionOptions ApplyIsolationLevel(TransactionOptions options, IsolationLevel isolationLevel)
        {
            return new TransactionOptions
            {
                Timeout = options.Timeout,
                IsolationLevel = isolationLevel
            };
        }
    }
}
