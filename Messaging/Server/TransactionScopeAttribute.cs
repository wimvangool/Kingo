using System.Globalization;
using System.Transactions;

namespace System.ComponentModel.Server
{
    /// <summary>
    /// This attribute can be applied to messages to specify how a <see cref="TransactionScope" /> should be created.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TransactionScopeAttribute : MessageHandlerModuleAttribute
    {
        private readonly TransactionScopeFactory _transactionScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute" /> class.
        /// </summary>
        /// <param name="scopeOption">The <see cref="TransactionScopeOption" /> to use.</param>        
        public TransactionScopeAttribute(TransactionScopeOption scopeOption)
        {
            _transactionScopeFactory = new TransactionScopeFactory(
                scopeOption,
                TransactionManager.DefaultTimeout,
                IsolationLevel.Unspecified,
                EnterpriseServicesInteropOption.None);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute" /> class.
        /// </summary>
        /// <param name="scopeOption">The <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The timeout for the transaction.</param>        
        public TransactionScopeAttribute(TransactionScopeOption scopeOption, string timeout)
        {
            _transactionScopeFactory = new TransactionScopeFactory(
                scopeOption,
                TimeSpan.Parse(timeout, CultureInfo.InvariantCulture),
                IsolationLevel.Unspecified,
                EnterpriseServicesInteropOption.None);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionScopeAttribute" /> class.
        /// </summary>
        /// <param name="scopeOption">The <see cref="TransactionScopeOption" /> to use.</param>
        /// <param name="timeout">The timeout for the transaction.</param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel" /> to use.</param>
        /// <param name="interopOption">The <see cref="EnterpriseServicesInteropOption" /> to use.</param>
        public TransactionScopeAttribute(TransactionScopeOption scopeOption, string timeout, IsolationLevel isolationLevel, EnterpriseServicesInteropOption interopOption)
        {
            _transactionScopeFactory = new TransactionScopeFactory(
                scopeOption,
                TimeSpan.Parse(timeout, CultureInfo.InvariantCulture),
                isolationLevel,
                interopOption);
        }

        /// <summary>
        /// Creates and returns a new <see cref="TransactionScope" /> based on the configured properties.
        /// </summary>
        /// <returns>A new <see cref="TransactionScope" />.</returns>
        public TransactionScope CreateTransactionScope()
        {
            return _transactionScopeFactory.CreateTransactionScope();
        }
    }
}
