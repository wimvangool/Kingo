using System;

namespace Kingo.Messaging.Domain
{
    /// <summary>
    /// Represents a <see cref="ITypeToContractMap" /> that has been implemented through a set of delegates.
    /// </summary>
    public sealed class DelegateTypeToContractMap : ITypeToContractMap
    {
        private readonly Func<Type, string> _getContract;
        private readonly Func<string, Type> _getType;    
    
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateTypeToContractMap" /> class.
        /// </summary>
        /// <param name="getContract">
        /// Delegate that returns a contract based on a specific type.
        /// </param>
        /// <param name="getType">
        /// Delegate that returns a type base on a specific contract.
        /// </param>
        public DelegateTypeToContractMap(Func<Type, string> getContract, Func<string, Type> getType)
        {
            if (getContract == null)
            {
                throw new ArgumentNullException("getContract");
            }
            if (getType == null)
            {
                throw new ArgumentNullException("getType");
            }
            _getContract = getContract;
            _getType = getType;
        }

        /// <inheritdoc />
        public string GetContract(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return _getContract.Invoke(type);
        }

        /// <inheritdoc />
        public Type GetType(string contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException("contract");
            }
            return _getType.Invoke(contract);
        }
    }
}
