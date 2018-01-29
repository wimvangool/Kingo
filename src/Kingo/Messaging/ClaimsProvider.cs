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
        public ClaimsProvider(ClaimsPrincipal principal)
        {
            _principal = principal ?? CreatePrincipal(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProvider" /> class.
        /// </summary>
        /// <param name="principal">The principal this provider will be based on.</param>        
        public ClaimsProvider(IPrincipal principal)
        {
            _principal = principal as ClaimsPrincipal ?? CreatePrincipal(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProvider" /> class.
        /// </summary>
        /// <param name="claims">
        /// A collection of claims to be managed by this provider, where each key represents
        /// the claim-type and each value represents the claim-value.
        /// </param>
        public ClaimsProvider(IEnumerable<KeyValuePair<string, string>> claims) :
            this(claims.EnsureNotNull().Select(pair => new Claim(pair.Key, pair.Value))) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProvider" /> class.
        /// </summary>
        /// <param name="claims">A collection of claims to be managed by this provider.</param>        
        public ClaimsProvider(IEnumerable<Claim> claims)
        {
            _principal = CreatePrincipal(claims);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProvider" /> class.
        /// </summary>
        /// <param name="claims">A collection of claims to be managed by this provider.</param>
        public ClaimsProvider(params Claim[] claims)
        {
            _principal = CreatePrincipal(claims);
        }        

        private static ClaimsPrincipal CreatePrincipal(IEnumerable<Claim> claims) =>
            new ClaimsPrincipal(new[] { new ClaimsIdentity(claims.EnsureNotNull()) });        

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
