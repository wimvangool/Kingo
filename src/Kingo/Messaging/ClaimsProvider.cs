using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Kingo.Resources;

namespace Kingo.Messaging
{
    internal sealed class ClaimsProvider : IClaimsProvider
    {
        private readonly ClaimsPrincipal _principal;

        public ClaimsProvider(IPrincipal principal)
        {
            _principal = principal as ClaimsPrincipal ?? new ClaimsPrincipal(principal);
        }

        public IEnumerable<Claim> Claims =>
            _principal.Claims;

        public bool HasClaim(string type, Func<IEnumerable<Claim>, Claim> filter = null) =>
            FindClaimOrNull(type, filter) != null;                 

        public Claim FindClaim(string type, Func<IEnumerable<Claim>, Claim> filter = null)
        {
            if (TryFindClaim(type, out Claim claim, filter))
            {
                return claim;
            }
            throw NewClaimNotFoundException(_principal.Identity, type);
        }                

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
