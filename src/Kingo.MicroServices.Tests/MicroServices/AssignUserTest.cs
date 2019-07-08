using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices
{
    [TestClass]
    public sealed class AssignUserTest : MicroProcessorTest<MicroProcessor>
    {
        private IMicroProcessor _processor;

        [TestInitialize]
        public void Setup()
        {
            _processor = CreateProcessor();
        }        

        [TestMethod]
        public async Task User_HasNoIdentity_IfNoUserIsSpecificallyAssignedToRequest()
        {
            await _processor.ExecuteCommandAsync((message, context) =>
            {
                Assert.IsNotNull(context.User);
                Assert.IsNull(context.User.Identity);
            }, new object());
        }

        [TestMethod]
        public async Task User_HasExpectedRolesAndIdentities_IfUserIsGenericPrincipal()
        {
            var userName = Guid.NewGuid().ToString();
            var roles = new [] { "A", "B" };
            var user = new GenericPrincipal(new GenericIdentity(userName), roles);

            using (_processor.AssignUser(user))
            {
                await _processor.ExecuteCommandAsync((message, context) =>
                {
                    Assert.IsNotNull(context.User);
                    Assert.IsNotNull(context.User.Identity);
                    Assert.IsTrue(context.User.Identity.IsAuthenticated);

                    Assert.IsTrue(context.User.IsInRole(roles[0]));
                    Assert.IsTrue(context.User.IsInRole(roles[1]));                    

                    // The default claims contains one Name-claims and one claim for each role.
                    Assert.AreEqual(1 + roles.Length, context.User.Claims.Count());
                }, new object());
            }
        }

        [TestMethod]
        public async Task User_HasExpectedRolesAndIdentities_IfUserIsClaimsPrincipal()
        {                      
            var claims = new []
            {
                new Claim("X", "x"),
                new Claim("Y", "y"),
                new Claim("Z", "z")
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));            

            using (_processor.AssignUser(user))
            {
                await _processor.ExecuteCommandAsync((message, context) =>
                {
                    Assert.AreNotSame(context.User, user);
                    Assert.IsNotNull(context.User);
                    Assert.IsNotNull(context.User.Identity);
                    Assert.IsFalse(context.User.Identity.IsAuthenticated);                    
                    
                    Assert.AreEqual(claims.Length, context.User.Claims.Count());
                    Assert.IsTrue(claims.All(claim => context.User.HasClaim(claim.Type, claim.Value)));                    
                }, new object());
            }

            await _processor.ExecuteCommandAsync((message, context) =>
            {
                Assert.IsNotNull(context.User);
                Assert.IsNull(context.User.Identity);
            }, new object());
        }        
    }
}
