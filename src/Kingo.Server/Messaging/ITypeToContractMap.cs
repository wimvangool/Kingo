using System;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a mapping between the types of events and snapshots
    /// and a contract or schema, which can be used to support versioning and upcasting of these types.
    /// </summary>
    public interface ITypeToContractMap
    {
        /// <summary>
        /// Retrieves the contract to which the specified <paramref name="type "/> is mapped.
        /// </summary>
        /// <param name="type">The type to get the contract for.</param>
        /// <returns>The contract to which the type is mapped.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No mapping was found for the specified <paramref name="type"/>.
        /// </exception>
        string GetContract(Type type);

        /// <summary>
        /// Retrieved the type to which the specified <paramref name="contract"/> is mapped.
        /// </summary>
        /// <param name="contract">The contract to get the type for.</param>
        /// <returns>The type to which the contract is mapped.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="contract"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// No mapping was found for the specified <paramref name="contract"/>.
        /// </exception>
        Type GetType(string contract);
    }
}
