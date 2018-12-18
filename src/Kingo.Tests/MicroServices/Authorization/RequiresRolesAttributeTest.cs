using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Authorization
{
    [TestClass]
    public sealed class RequiresRolesAttributeTest
    {
        #region [====== MessageHandlers & Queries ======]

        private const string _RequiredRole = "Role-X";

        private sealed class MessageHandlerThatRequiresNoRoles : IMessageHandler<object>
        {
            [RequiresRoles]
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }

        private sealed class MessageHandlerThatRequiresCertainRoles : IMessageHandler<object>
        {
            [RequiresRoles(_RequiredRole)]
            public Task HandleAsync(object message, MessageHandlerContext context) =>
                Task.CompletedTask;
        }        

        private sealed class QueryThatRequiresNoRoles : IQuery<object>, IQuery<object, object>
        {
            [RequiresRoles]
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());

            [RequiresRoles]
            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
        }

        private sealed class QueryThatRequiresCertainRoles : IQuery<object>, IQuery<object, object>
        {
            [RequiresRoles(_RequiredRole)]
            public Task<object> ExecuteAsync(QueryContext context) =>
                Task.FromResult(new object());

            [RequiresRoles(_RequiredRole)]
            public Task<object> ExecuteAsync(object message, QueryContext context) =>
                Task.FromResult(message);
        }

        #endregion

        #region [====== RoleBasedPrincipal ======]

        private sealed class RoleBasedPrincipal : IPrincipal
        {
            private readonly RoleBasedIdentity _identity;
            private readonly string[] _roles;

            public RoleBasedPrincipal(bool isAuthenticated, params string[] roles)
            {
                _identity = new RoleBasedIdentity(isAuthenticated);
                _roles = roles;
            }

            public IIdentity Identity =>
                _identity;

            public bool IsInRole(string role) =>
                _roles.Any(roleOfPrincipal => roleOfPrincipal == role);
        }

        private sealed class RoleBasedIdentity : IIdentity
        {            
            public RoleBasedIdentity(bool isAuthenticated)
            {
                AuthenticationType = "Custom";
                IsAuthenticated = isAuthenticated;
                Name = nameof(RoleBasedIdentity);
            }

            public string AuthenticationType
            {
                get;
            }

            public bool IsAuthenticated
            {
                get;
            }

            public string Name
            {
                get;
            }
        }

        #endregion

        #region [====== HandleAsync ======]

        [TestMethod]
        public async Task HandleAsync_InvokesMessageHandler_IfPrincipalIsNotSet_And_NoRolesAreRequired()
        {
            Assert.AreEqual(1, await CreateProcessor(null).HandleAsync(new object(), new MessageHandlerThatRequiresNoRoles()));
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task HandleAsync_Throws_IfIdentityIsNotAuthenticated_And_SomeRolesAreRequired()
        {
            await CreateProcessor(new RoleBasedPrincipal(false, _RequiredRole)).HandleAsync(new object(), new MessageHandlerThatRequiresCertainRoles());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task HandleAsync_Throws_IfPrincipalIsNotInRequiredRole()
        {
            await CreateProcessor(new RoleBasedPrincipal(true)).HandleAsync(new object(), new MessageHandlerThatRequiresCertainRoles());
        }

        [TestMethod]
        public async Task HandleAsync_InvokesMessageHandler_IfPrincipalIsInRequiredRole()
        {
            Assert.AreEqual(1, await CreateProcessor(new RoleBasedPrincipal(true, _RequiredRole)).HandleAsync(new object(), new MessageHandlerThatRequiresNoRoles()));
        }

        #endregion

        #region [====== ExecuteAsync1 ======]

        [TestMethod]
        public async Task ExecuteAsync1_InvokesQuery_IfPrincipalIsNotSet_And_NoRolesAreRequired()
        {
            await CreateProcessor(null).ExecuteAsync(new QueryThatRequiresNoRoles());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync1_Throws_IfIdentityIsNotAuthenticated_And_SomeRolesAreRequired()
        {
            await CreateProcessor(new RoleBasedPrincipal(false, _RequiredRole)).ExecuteAsync(new QueryThatRequiresCertainRoles());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync1_Throws_IfPrincipalIsNotInRequiredRole()
        {
            await CreateProcessor(new RoleBasedPrincipal(true)).ExecuteAsync(new QueryThatRequiresCertainRoles());
        }

        [TestMethod]
        public async Task ExecuteAsync1_InvokesMessageHandler_IfPrincipalIsInRequiredRole()
        {
            await CreateProcessor(new RoleBasedPrincipal(true, _RequiredRole)).ExecuteAsync(new QueryThatRequiresCertainRoles());
        }

        #endregion

        #region [====== ExecuteAsync2 ======]

        [TestMethod]
        public async Task ExecuteAsync2_InvokesQuery_IfPrincipalIsNotSet_And_NoRolesAreRequired()
        {
            await CreateProcessor(null).ExecuteAsync(new object(), new QueryThatRequiresNoRoles());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync2_Throws_IfIdentityIsNotAuthenticated_And_SomeRolesAreRequired()
        {
            await CreateProcessor(new RoleBasedPrincipal(false, _RequiredRole)).ExecuteAsync(new object(), new QueryThatRequiresCertainRoles());
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedRequestException))]
        public async Task ExecuteAsync2_Throws_IfPrincipalIsNotInRequiredRole()
        {
            await CreateProcessor(new RoleBasedPrincipal(true)).ExecuteAsync(new object(), new QueryThatRequiresCertainRoles());
        }

        [TestMethod]
        public async Task ExecuteAsync2_InvokesMessageHandler_IfPrincipalIsInRequiredRole()
        {
            await CreateProcessor(new RoleBasedPrincipal(true, _RequiredRole)).ExecuteAsync(new object(), new QueryThatRequiresCertainRoles());
        }

        #endregion

        private static IMicroProcessor CreateProcessor(IPrincipal principal) =>
            new AuthorizationAttributeTestProcessor(principal);
    }
}
