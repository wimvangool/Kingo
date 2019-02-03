using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Authorization
{
    [TestClass]
    public sealed class RequiresClaimsAttributeTest
    {
        #region [====== MessageHandlers & Queries ======]

        private const string _RequiredClaim = "Claim-X";        

        private sealed class MessageHandlerThatRequiresNoClaims : IMessageHandler<object>
        {
            [RequiresClaims]
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandlerThatRequiresCertainClaims : IMessageHandler<object>
        {
            [RequiresClaims(_RequiredClaim)]
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class QueryThatRequiresNoClaims : IQuery<object>, IQuery<object, object>
        {
            [RequiresClaims]
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());

            [RequiresClaims]
            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
        }

        private sealed class QueryThatRequiresCertainClaims : IQuery<object>, IQuery<object, object>
        {
            [RequiresClaims(_RequiredClaim)]
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());

            [RequiresClaims(_RequiredClaim)]
            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
        }

        #endregion

        #region [====== ClaimsPrincipal ======]

        private sealed class CustomClaimsPrincipal : ClaimsPrincipal
        {
            public CustomClaimsPrincipal(bool isAuthenticated, params string[] claims) :
                base(new CustomClaimsIdentity(isAuthenticated, claims)) { }
        }

        private sealed class CustomClaimsIdentity : ClaimsIdentity
        {
            public CustomClaimsIdentity(bool isAuthenticated, params string[] claims) :
                base(CreateClaims(claims))
            {
                IsAuthenticated = isAuthenticated;
            }

            public override bool IsAuthenticated
            {
                get;
            }

            private static IEnumerable<Claim> CreateClaims(IEnumerable<string> claims) =>
                claims.Select(claim => new Claim(claim, string.Empty));
        }

        #endregion

        #region [====== HandleAsync ======]

        [TestMethod]
        public async Task HandleAsync_InvokesMessageHandler_IfPrincipalIsNotSet_And_NoRolesAreRequired()
        {                        
           Assert.AreEqual(1, await CreateProcessor(null).HandleAsync(new object(), new MessageHandlerThatRequiresNoClaims()));
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task HandleAsync_Throws_IfIdentityIsNotAuthenticated_And_SomeRolesAreRequired()
        {
            await CreateProcessor(new CustomClaimsPrincipal(false, _RequiredClaim)).HandleAsync(new object(), new MessageHandlerThatRequiresCertainClaims());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task HandleAsync_Throws_IfPrincipalIsNotInRequiredRole()
        {
            await CreateProcessor(new CustomClaimsPrincipal(true)).HandleAsync(new object(), new MessageHandlerThatRequiresCertainClaims());
        }

        [TestMethod]
        public async Task HandleAsync_InvokesMessageHandler_IfPrincipalIsInRequiredRole()
        {
            Assert.AreEqual(1, await CreateProcessor(new CustomClaimsPrincipal(true, _RequiredClaim)).HandleAsync(new object(), new MessageHandlerThatRequiresNoClaims()));
        }

        #endregion

        #region [====== ExecuteAsync1 ======]

        [TestMethod]
        public async Task ExecuteAsync1_InvokesQuery_IfPrincipalIsNotSet_And_NoRolesAreRequired()
        {
            await CreateProcessor(null).ExecuteAsync(new QueryThatRequiresNoClaims());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync1_Throws_IfIdentityIsNotAuthenticated_And_SomeRolesAreRequired()
        {
            await CreateProcessor(new CustomClaimsPrincipal(false, _RequiredClaim)).ExecuteAsync(new QueryThatRequiresCertainClaims());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync1_Throws_IfPrincipalIsNotInRequiredRole()
        {
            await CreateProcessor(new CustomClaimsPrincipal(true)).ExecuteAsync(new QueryThatRequiresCertainClaims());
        }

        [TestMethod]
        public async Task ExecuteAsync1_InvokesMessageHandler_IfPrincipalIsInRequiredRole()
        {
            await CreateProcessor(new CustomClaimsPrincipal(true, _RequiredClaim)).ExecuteAsync(new QueryThatRequiresCertainClaims());
        }

        #endregion

        #region [====== ExecuteAsync2 ======]

        [TestMethod]
        public async Task ExecuteAsync2_InvokesQuery_IfPrincipalIsNotSet_And_NoRolesAreRequired()
        {
            await CreateProcessor(null).ExecuteAsync(new object(), new QueryThatRequiresNoClaims());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync2_Throws_IfIdentityIsNotAuthenticated_And_SomeRolesAreRequired()
        {
            await CreateProcessor(new CustomClaimsPrincipal(false, _RequiredClaim)).ExecuteAsync(new object(), new QueryThatRequiresCertainClaims());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync2_Throws_IfPrincipalIsNotInRequiredRole()
        {
            await CreateProcessor(new CustomClaimsPrincipal(true)).ExecuteAsync(new object(), new QueryThatRequiresCertainClaims());
        }

        [TestMethod]
        public async Task ExecuteAsync2_InvokesMessageHandler_IfPrincipalIsInRequiredRole()
        {
            await CreateProcessor(new CustomClaimsPrincipal(true, _RequiredClaim)).ExecuteAsync(new object(), new QueryThatRequiresCertainClaims());
        }

        #endregion

        private static IMicroProcessor CreateProcessor(IPrincipal principal) =>
            new AuthorizationAttributeTestProcessor(principal);
    }
}
