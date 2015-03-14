﻿using System.Transactions;

namespace System.ComponentModel.Server.Transactions
{
    /// <summary>
    /// Represents a module that creates a <see cref="TransactionScope" /> before calling the next handler.
    /// </summary>        
    public class TransactionScopeModule : MessageHandlerModule<ITransactionScopeFactory>       
    {                
        private readonly ITransactionScopeFactory _defaultFactory;               

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeModule" /> class.
        /// </summary>              
        /// <param name="scopeOption">The default <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The default timeout to use.</param>
        /// <param name="isolationLevel">
        /// The default <see cref="IsolationLevel" /> to use. If <see cref="IsolationLevel.Unspecified" /> is specified,
        /// the actual <see cref="IsolationLevel" /> applied will be that of any running <see cref="Transaction" /> or
        /// of the .NET Framework's default <see cref="IsolationLevel" /> setting, <see cref="IsolationLevel.Serializable"/>.
        /// </param>        
        /// <param name="interopOption">The default <see cref="EnterpriseServicesInteropOption" /> to use.</param> 
        public TransactionScopeModule(TransactionScopeOption scopeOption, TimeSpan timeout, IsolationLevel isolationLevel = IsolationLevel.Unspecified, EnterpriseServicesInteropOption interopOption = EnterpriseServicesInteropOption.None)
            : this(new TransactionScopeFactory(scopeOption, timeout, isolationLevel, interopOption)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeModule" /> class.
        /// </summary>        
        /// <param name="defaultFactory">
        /// The default <see cref="ITransactionScopeFactory" /> to use to create a new <see cref="TransactionScope"/>.
        /// </param>        
        public TransactionScopeModule(ITransactionScopeFactory defaultFactory = null)            
        {            
            _defaultFactory = defaultFactory ?? new TransactionScopeFactory();
        }                

        /// <inheritdoc />
        protected override ITransactionScopeFactory DefaultStrategy
        {
            get { return _defaultFactory; }
        }

        /// <inheritdoc />
        protected override void InvokeHandler(IMessageHandler handler, ITransactionScopeFactory strategy)
        {
            using (var transactionScope = strategy.CreateTransactionScope())
            {
                handler.Invoke();
                transactionScope.Complete();
            }
        }        
    }
}
