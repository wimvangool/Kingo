using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for the <see cref="IClaimsProvider" /> interface.
    /// </summary>
    public static class ClaimsProviderExtensions
    {
        /// <summary>
        /// Indicates whether or not a claim with the specified <paramref name="type" /> is present and, if specified, if
        /// at least one of those claims satisfies the specified <paramref name="predicate" />.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to check.</param>
        /// <param name="predicate">
        /// Optional predicate that can be used to check if any of the claims matches certain criteria.
        /// </param>
        /// <returns>
        /// <c>true</c> if at least one claim with the specified <paramref name="type"/> is present and matches the specified <paramref name="predicate" />;
        /// otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool HasClaim(this IClaimsProvider provider, string type, Func<Claim, bool> predicate)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }            
            return provider.HasClaim(type, ToFilter(predicate));
        }

        /// <summary>
        /// Retrieves the value of a claim that has the specified <paramref name="type" />. If multiple claims of the specified <paramref name="type" />
        /// are found, the value of the first one that satisfies the <paramref name="predicate" /> is returned. If <paramref name="predicate" /> is <c>null</c>,
        /// the value of the first claim of the specified <paramref name="type"/> will be returned.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="predicate">
        /// Optional predicate that can be used to select a claim that matches specific criteria.
        /// </param>
        /// <returns>The value of the claim.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ClaimNotFoundException">
        /// No claim of the specified <paramref name="type"/> was found.
        /// </exception>
        public static string FindClaimValue(this IClaimsProvider provider, string type, Func<Claim, bool> predicate) =>
            FindClaimValue(provider, type, ToFilter(predicate));

        /// <summary>
        /// Retrieves the value of a claim that has the specified <paramref name="type" />. If multiple claims of the specified <paramref name="type" />
        /// are found, the specified <paramref name="filter" /> is used to select the desired one. If <paramref name="filter" /> is <c>null</c>, the value
        /// of the first claim of the specified <paramref name="type"/> will be returned.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="filter">
        /// Optional filter that can be used to select a single claim from potentially many claims of a specific type. If this parameter is specified
        /// and returns <c>null</c> when invoked, this method behaves as if no claim was found and throws a <see cref="ClaimNotFoundException" />.
        /// </param>
        /// <returns>The value of the claim.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ClaimNotFoundException">
        /// No claim of the specified <paramref name="type"/> was found.
        /// </exception>
        public static string FindClaimValue(this IClaimsProvider provider, string type, Func<IEnumerable<Claim>, Claim> filter = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return provider.FindClaim(type, filter).Value;
        }

        /// <summary>
        /// Retrieves the first claim that has the specified <paramref name="type" /> and satisfies the specified <paramref name="predicate" />.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="predicate">
        /// Optional predicate that can be used to select a claim that matches specific criteria.
        /// </param>
        /// <returns>The value of the claim.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ClaimNotFoundException">
        /// No claim of the specified <paramref name="type"/> was found.
        /// </exception>
        public static Claim FindClaim(this IClaimsProvider provider, string type, Func<Claim, bool> predicate)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return provider.FindClaim(type, ToFilter(predicate));
        }            

        /// <summary>
        /// Attempts to retrieve the value of a claim that was found that has the specified <paramref name="type" />.
        /// If multiple claims of the specified <paramref name="type" /> are found, the value of the first one that satisfies
        /// the <paramref name="predicate" /> is returned. If <paramref name="predicate" /> is <c>null</c>,
        /// the value of the first claim of the specified <paramref name="type"/> will be returned.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="value">If the claim was found, this parameter will refer to the value of the claim.</param>
        /// <param name="predicate">
        /// Optional predicate that can be used to select a claim that matches specific criteria.
        /// </param>
        /// <returns><c>true</c> is the claim was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryFindClaimValue(this IClaimsProvider provider, string type, out string value, Func<Claim, bool> predicate) =>
            TryFindClaimValue(provider, type, out value, ToFilter(predicate));

        /// <summary>
        /// Attempts to retrieve the value of a claim that was found that has the specified <paramref name="type" />.
        /// If multiple claims of the specified <paramref name="type" /> are found, the specified <paramref name="filter" />
        /// is used to select the desired one. If <paramref name="filter" /> is <c>null</c>, the value of the first claim of
        /// the specified <paramref name="type" /> will be returned.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="value">If the claim was found, this parameter will refer to the value of the claim.</param>
        /// <param name="filter">
        /// Optional filter that can be used to select a single claim from potentially many claims of a specific type. If this parameter is specified
        /// and returns <c>null</c> when invoked, this method behaves as if no claim was found and throws a <see cref="ClaimNotFoundException" />.
        /// </param>
        /// <returns><c>true</c> is the claim was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryFindClaimValue(this IClaimsProvider provider, string type, out string value, Func<IEnumerable<Claim>, Claim> filter = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            if (provider.TryFindClaim(type, out Claim claim, filter))
            {
                value = claim.Value;
                return true;
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Attempts to retrieve a claim that was found that has the specified <paramref name="type" /> and satisfies the specified
        /// <paramref name="predicate" />.
        /// </summary>
        /// <param name="provider">A claims provider.</param>
        /// <param name="type">Type of the claim to retrieve.</param>
        /// <param name="claim">If the claim was found, this parameter will refer to the claim.</param>
        /// <param name="predicate">
        /// Optional predicate that can be used to select a claim that matches specific criteria.
        /// </param>
        /// <returns><c>true</c> is the claim was found; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="provider"/> or <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool TryFindClaim(this IClaimsProvider provider, string type, out Claim claim, Func<Claim, bool> predicate)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return provider.TryFindClaim(type, out claim, ToFilter(predicate));
        }

        private static Func<IEnumerable<Claim>, Claim> ToFilter(Func<Claim, bool> predicate)
        {
            if (predicate == null)
            {
                return claims => claims.FirstOrDefault();
            }
            return claims => claims.FirstOrDefault(predicate);
        }             
    }
}
