using System.Globalization;
using System.Transactions;
using Microsoft.SqlServer.Server;

namespace System.ComponentModel.Server.Transactions
{
    /// <summary>
    /// This attribute can be applied to messages to specify how a <see cref="TransactionScope" /> should be created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TransactionScopeAttribute : Attribute, ITransactionScopeFactory
    {
        private readonly Lazy<TransactionScopeFactory> _transactionScopeFactory;
        private readonly TransactionScopeOption _scopeOption;
        private readonly string _timeout;
        private readonly IsolationLevel _isolationLevel;
        private readonly EnterpriseServicesInteropOption _interopOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute" /> class.
        /// </summary>
        /// <param name="scopeOption">The <see cref="TransactionScopeOption" /> to use.</param>        
        public TransactionScopeAttribute(TransactionScopeOption scopeOption)
            : this(scopeOption, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute" /> class.
        /// </summary>
        /// <param name="scopeOption">The <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The timeout for the transaction.</param>        
        public TransactionScopeAttribute(TransactionScopeOption scopeOption, string timeout)
            : this(scopeOption, timeout, IsolationLevel.Unspecified, EnterpriseServicesInteropOption.None) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute" /> class.
        /// </summary>
        /// <param name="scopeOption">The <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The timeout for the transaction.</param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use.</param>
        /// <param name="interopOption">The <see cref="EnterpriseServicesInteropOption" /> to use.</param>
        public TransactionScopeAttribute(TransactionScopeOption scopeOption, string timeout, IsolationLevel isolationLevel, EnterpriseServicesInteropOption interopOption)
        {
            _transactionScopeFactory = new Lazy<TransactionScopeFactory>(CreateTransactionScopeFactory, true);
            _scopeOption = scopeOption;
            _timeout = timeout;
            _isolationLevel = isolationLevel;
            _interopOption = interopOption;
        }

        private TransactionScopeFactory CreateTransactionScopeFactory()
        {
            return new TransactionScopeFactory(ScopeOption, Parse(Timeout), IsolationLevel, InteropOption);
        }

        /// <summary>
        /// Returns the <see cref="TransactionScopeOption" /> that is used for the <see cref="TransactionScope" /> to be created.
        /// </summary>
        public TransactionScopeOption ScopeOption
        {
            get { return _scopeOption; }
        }

        /// <summary>
        /// Returns the timeout that is used for the <see cref="TransactionScope" /> to be created.
        /// </summary>
        public string Timeout
        {
            get { return _timeout; }
        }

        /// <summary>
        /// Returns the <see cref="IsolationLevel" /> that is used for the <see cref="TransactionScope" /> to be created.
        /// </summary>
        public IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
        }

        /// <summary>
        /// Returns the <see cref="EnterpriseServicesInteropOption" /> that is used for the <see cref="TransactionScope" /> to be created.
        /// </summary>
        public EnterpriseServicesInteropOption InteropOption
        {
            get { return _interopOption; }
        }
        
        TransactionScope ITransactionScopeFactory.CreateTransactionScope()
        {
            return _transactionScopeFactory.Value.CreateTransactionScope();
        }

        private static TimeSpan Parse(string timeout)
        {
            return timeout == null ? TransactionManager.DefaultTimeout : TimeSpan.Parse(timeout);
        }
    }
}
