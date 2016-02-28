using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Kingo.Resources;

namespace Kingo.Messaging
{

    /// <summary>
    /// This exception is thrown when a two or more types were mapped to the same contract
    /// inside a <see cref="ITypeToContractMap" />.
    /// </summary>
    [Serializable]
    public sealed class TypeToContractMapException : InvalidOperationException
    {
        private const string _Type1Key = "_type1";
        private readonly Type _type1;

        private const string _Type2Key = "_type2";
        private readonly Type _type2;

        private const string _ContractKey = "_contract";
        private readonly string _contract;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeToContractMapException" /> class.
        /// </summary>
        /// <param name="type1">The first type that maps to the specified <paramref name="contract"/>.</param>
        /// <param name="type2">The second type that maps to the specified <paramref name="contract"/>.</param>
        /// <param name="contract">The contract that both types are mapped to.</param>
        /// <param name="message">The exception message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type1"/>, <paramref name="type2"/> or <paramref name="contract"/> is <c>null</c>.
        /// </exception>
        public TypeToContractMapException(Type type1, Type type2, string contract, string message = null)
            : base(message ?? CreateMessage(type1, type2, contract))
        {
            if (type1 == null)
            {
                throw new ArgumentNullException(nameof(type1));
            }
            if (type2 == null)
            {
                throw new ArgumentNullException(nameof(type2));
            }
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }
            _type1 = type1;
            _type2 = type2;
            _contract = contract;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeToContractMapException" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        private TypeToContractMapException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _type1 = (Type) info.GetValue(_Type1Key, typeof(Type));
            _type2 = (Type) info.GetValue(_Type2Key, typeof(Type));
            _contract = info.GetString(_ContractKey);
        }

        /// <inheritdoc />
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(_Type1Key, _type1);
            info.AddValue(_Type2Key, _type2);
            info.AddValue(_ContractKey, _contract);
        }        

        /// <summary>
        /// The first type that maps to the specified <see cref="Contract" />.
        /// </summary>
        public Type Type1
        {
            get { return _type1; }
        }

        /// <summary>
        /// The second type that maps to the specified <see cref="Contract" />.
        /// </summary>
        public Type Type2
        {
            get { return _type2; }
        }

        /// <summary>
        /// The contract that both types are mapped to.
        /// </summary>
        public string Contract
        {
            get { return _contract; }
        }

        private static string CreateMessage(Type type1, Type type2, string contract)
        {
            return string.Format(ExceptionMessages.TypeToContractMap_SharedContract, type1, type2, contract);            
        }
    }
}
