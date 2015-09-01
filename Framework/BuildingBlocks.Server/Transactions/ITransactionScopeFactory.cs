using System.Transactions;

namespace Kingo.BuildingBlocks.Transactions
{
    /// <summary>
    /// When implemented by a class, represents a factory for <see cref="TransactionScope">TransactionScopes</see>.
    /// </summary>
    public interface ITransactionScopeFactory
    {
        /// <summary>
        /// Creates and returns a new <see cref="TransactionScope" />.
        /// </summary>
        /// <returns>A new <see cref="TransactionScope" />.</returns>
        TransactionScope CreateTransactionScope();
    }
}
