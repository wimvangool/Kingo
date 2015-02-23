using System.Transactions;

namespace System.ComponentModel.Server
{
    internal sealed class TransactionScopeFactory
    {
        private readonly TransactionScopeOption _scopeOption;
        private readonly TransactionOptions _transactionOptions;
        private readonly EnterpriseServicesInteropOption _interopOption;

        internal TransactionScopeFactory(TransactionScopeOption scopeOption, TimeSpan timeout, IsolationLevel isolationLevel, EnterpriseServicesInteropOption interopOption)
        {
            _scopeOption = scopeOption;
            _transactionOptions = new TransactionOptions()
            {
                Timeout = timeout,
                IsolationLevel = isolationLevel
            };
            _interopOption = interopOption;
        }

        internal TransactionScope CreateTransactionScope()
        {               
            if (_transactionOptions.IsolationLevel == IsolationLevel.Unspecified)
            {
                var currentTransaction = Transaction.Current;
                if (currentTransaction == null || _scopeOption == TransactionScopeOption.RequiresNew)
                {                    
                    return new TransactionScope(_scopeOption, ApplyIsolationLevel(_transactionOptions, IsolationLevel.ReadCommitted), _interopOption);
                }
                return new TransactionScope(_scopeOption, _transactionOptions.Timeout);
            }
            return new TransactionScope(_scopeOption, _transactionOptions, _interopOption);        
        }

        private static TransactionOptions ApplyIsolationLevel(TransactionOptions options, IsolationLevel isolationLevel)
        {
            return new TransactionOptions()
            {
                Timeout = options.Timeout,
                IsolationLevel = isolationLevel
            };
        }
    }
}
