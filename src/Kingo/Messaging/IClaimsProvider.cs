using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Kingo.Messaging
{
    /// <summary>
    /// When implemented by a class, represents a provider of claims from an associated <see cref="ClaimsPrincipal" />.
    /// </summary>
    public interface IClaimsProvider
    {
        /// <summary>
        /// Returns all claims known to this provider.
        /// </summary>
        IEnumerable<Claim> Claims
        {
            get;
        }

        /// <summary>
        /// Indicates whether or not a claim with the specified <paramref name="type" /> is present and, if specified, if
        /// at least one of those claims satisfies the specified <paramref name="filter" />.
        /// </summary>
        /// <param name="type">Type of the claim to check.</param>
        /// <param name="filter">
        /// Optional filter that can be used to select a single claim from potentially many claims of a specific type. If this parameter is specified
        /// and returns <c>null</c> when invoked, this method will return <c>false</c> as if no claim was found.
        /// </param>
        /// <returns>
        /// <c>true</c> if at least one claim with the specified <paramref name="type"/> is present and
        /// is selected by the <paramref name="filter" />, if specified; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        bool HasClaim(string type, Func<IEnumerable<Claim>, Claim> filter = null);        

        /// <summary>
        /// Retrieves a claim that has the specified <paramref name="type" />. If multiple claims of the specified <paramref name="type" />
        /// are found, the specified <paramref name="filter" /> is used to select the desired one. If <paramref name="filter" /> is <c>null</c>,
        /// the first claim of the specified <paramref name="type" /> will be returned.
        /// </summary>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="filter">
        /// Optional filter that can be used to select a single claim from potentially many claims of a specific type. If this parameter is specified
        /// and returns <c>null</c> when invoked, this method behaves as if no claim was found and throws a <see cref="ClaimNotFoundException" />.
        /// </param>
        /// <returns>The value of the claim.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ClaimNotFoundException">
        /// No claim of the specified <paramref name="type"/> was found, or none of the matching claims made it through the specified <paramref name="filter"/>.
        /// </exception>
        Claim FindClaim(string type, Func<IEnumerable<Claim>, Claim> filter = null);        

        /// <summary>
        /// Attempts to retrieve the first claim that was found that has the specified <paramref name="type" />.
        /// If multiple claims of the specified <paramref name="type" /> are found, the specified <paramref name="filter" />
        /// is used to select the desired one. If <paramref name="filter" /> is <c>null</c>, the first claim of the specified
        /// <paramref name="type" /> will be returned.
        /// </summary>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="claim">If the claim was found, this parameter will refer to the claim.</param>
        /// <param name="filter">
        /// Optional filter that can be used to select a single claim from potentially many claims of a specific type. If this parameter is specified
        /// and returns <c>null</c> when invoked, this method behaves as if no claim was found and throws a <see cref="ClaimNotFoundException" />.
        /// </param>
        /// <returns><c>true</c> is the claim was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is <c>null</c>.
        /// </exception>
        bool TryFindClaim(string type, out Claim claim, Func<IEnumerable<Claim>, Claim> filter = null);
    }
}
