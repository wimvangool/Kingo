using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Kingo.Resources;

namespace Kingo.Messaging
{
    /// <summary>
    /// Provides an implementation of the <see cref="IClaimsProvider" /> class.
    /// </summary>
    public sealed class ClaimsProvider : IClaimsProvider
    {
        private readonly ClaimsPrincipal _principal;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProvider" /> class.
        /// </summary>
        /// <param name="principal">The principal this provider will be based on.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="principal" /> is <c>null</c>.
        /// </exception>
        public ClaimsProvider(ClaimsPrincipal principal)
        {
            _principal = principal ?? throw new ArgumentNullException(nameof(principal));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProvider" /> class.
        /// </summary>
        /// <param name="principal">The principal this provider will be based on.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="principal" /> is <c>null</c>.
        /// </exception>
        public ClaimsProvider(IPrincipal principal)
        {
            _principal = principal as ClaimsPrincipal ?? new ClaimsPrincipal(principal);
        }

        /// <inheritdoc />
        public IEnumerable<Claim> Claims =>
            _principal.Claims;

        /// <inheritdoc />
        public bool HasClaim(string type, Func<IEnumerable<Claim>, Claim> filter = null) =>
            FindClaimOrNull(type, filter) != null;

        /// <inheritdoc />
        public Claim FindClaim(string type, Func<IEnumerable<Claim>, Claim> filter = null)
        {
            if (TryFindClaim(type, out Claim claim, filter))
            {
                return claim;
            }
            throw NewClaimNotFoundException(_principal.Identity, type);
        }

        /// <inheritdoc />
        public bool TryFindClaim(string type, out Claim claim, Func<IEnumerable<Claim>, Claim> filter = null) =>
            (claim = FindClaimOrNull(type, filter)) != null;

        private Claim FindClaimOrNull(string type, Func<IEnumerable<Claim>, Claim> filter) =>
            EnsureFilter(filter).Invoke(_principal.FindAll(type));

        private static Func<IEnumerable<Claim>, Claim> EnsureFilter(Func<IEnumerable<Claim>, Claim> filter) =>
            filter ?? (claims => claims.FirstOrDefault());

        internal static Exception NewClaimNotFoundException(IIdentity identity, string type)
        {
            var messageFormat = ExceptionMessages.ClaimsProvider_ClaimNotFound;
            var message = string.Format(messageFormat, identity?.Name, type);
            return new ClaimNotFoundException(type, message);
        }
    }
}
