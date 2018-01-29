using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging
{
    [TestClass]
    public sealed class ClaimsProviderTest
    {
        private const string _ValueTypeInt = nameof(Int32);
        private const string _ValueTypeSingle = nameof(Single);

        private static readonly Claim _ClaimA = new Claim(nameof(_ClaimA), Guid.NewGuid().ToString("N"));
        private static readonly Claim _ClaimB_OfType_Int = new Claim(nameof(_ClaimB_OfType_Int), "0", _ValueTypeInt);
        private static readonly Claim _ClaimB_OfType_Single = new Claim(nameof(_ClaimB_OfType_Int), "0.0", _ValueTypeSingle);       

        #region [====== HasClaim ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasClaim_Throws_IfTypeIsNull()
        {
            var provider = CreateProvider();

            provider.HasClaim(null);
        }

        [TestMethod]
        public void HasClaim_ReturnsFalse_IfPrincipalIsNoClaimsPrincipal()
        {
            var provider = new ClaimsProvider(Thread.CurrentPrincipal);

            Assert.IsFalse(provider.HasClaim(_ClaimA.Type));
        }

        [TestMethod]
        public void HasClaim_ReturnsFalse_IfPrincipalHasNoClaims()
        {
            var provider = CreateProvider();

            Assert.IsFalse(provider.HasClaim(_ClaimA.Type));
        }

        [TestMethod]
        public void HasClaim_ReturnsFalse_IfPrincipalHasNoClaimOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimB_OfType_Int);

            Assert.IsFalse(provider.HasClaim(_ClaimA.Type));
        }

        [TestMethod]
        public void HasClaim_ReturnsFalse_IfPrincipalHasOneClaimOfSpecifiedType_But_ClaimDoesNotSatisfyPredicate()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);

            Assert.IsFalse(provider.HasClaim(_ClaimA.Type, claim => false));
        }

        [TestMethod]
        public void HasClaim_ReturnsTrue_IfPrincipalHasOneClaimOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimA);

            Assert.IsTrue(provider.HasClaim(_ClaimA.Type));
        }

        [TestMethod]
        public void HasClaim_ReturnsTrue_IfPrincipalHasManyClaimsOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);

            Assert.IsTrue(provider.HasClaim(_ClaimB_OfType_Int.Type));
        }

        [TestMethod]
        public void HasClaim_ReturnsTrue_IfPrincipalHasManyClaimsOfSpecifiedType_And_AtLeastOneSatisfiesThePredicate()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);

            Assert.IsTrue(provider.HasClaim(_ClaimB_OfType_Int.Type, claim => claim.ValueType == _ValueTypeSingle));
        }

        #endregion

        #region [====== TryFindClaim ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryFindClaim_Throws_IfTypeIsNull()
        {
            var provider = CreateProvider();

            provider.TryFindClaim(null, out Claim claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsFalse_IfPrincipalIsNoClaimsPrincipal()
        {
            var provider = new ClaimsProvider(Thread.CurrentPrincipal);
            Claim claim;

            Assert.IsFalse(provider.TryFindClaim(_ClaimA.Type, out claim));
            Assert.IsNull(claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsFalse_IfPrincipalHasNoClaims()
        {
            var provider = CreateProvider();
            Claim claim;

            Assert.IsFalse(provider.TryFindClaim(_ClaimA.Type, out claim));
            Assert.IsNull(claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsFalse_IfPrincipalHasNoClaimOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimB_OfType_Int);
            Claim claim;

            Assert.IsFalse(provider.TryFindClaim(_ClaimA.Type, out claim));
            Assert.IsNull(claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsFalse_IfPrincipalHasOneClaimOfSpecifiedType_But_ClaimDoesNotSatisfyPredicate()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);
            Claim claim;

            Assert.IsFalse(provider.TryFindClaim(_ClaimA.Type, out claim, claims => null));
            Assert.IsNull(claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsTrue_IfPrincipalHasOneClaimOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimA);
            Claim claim;

            Assert.IsTrue(provider.TryFindClaim(_ClaimA.Type, out claim));
            Assert.IsNotNull(claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsTrue_IfPrincipalHasManyClaimsOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);
            Claim claim;

            Assert.IsTrue(provider.TryFindClaim(_ClaimB_OfType_Int.Type, out claim));
            Assert.IsNotNull(claim);
        }

        [TestMethod]
        public void TryFindClaim_ReturnsTrue_IfPrincipalHasManyClaimsOfSpecifiedType_And_AtLeastOneSatisfiesThePredicate()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);            
            Claim claim;

            Assert.IsTrue(provider.TryFindClaim(_ClaimB_OfType_Int.Type, out claim, c => c.ValueType == _ValueTypeSingle));
            Assert.IsNotNull(claim);
        }

        #endregion

        #region [====== FindClaim ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindClaim_Throws_IfTypeIsNull()
        {
            var provider = CreateProvider();

            provider.FindClaim(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void FindClaim_Throws_IfPrincipalIsNoClaimsPrincipal()
        {
            var provider = new ClaimsProvider(Thread.CurrentPrincipal);

            provider.FindClaim(_ClaimA.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void FindClaim_Throws_IfPrincipalHasNoClaims()
        {
            var provider = CreateProvider();

            provider.FindClaim(_ClaimA.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void FindClaim_ReturnsFalse_IfPrincipalHasNoClaimOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimB_OfType_Int);

            provider.FindClaim(_ClaimA.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(ClaimNotFoundException))]
        public void FindClaim_ReturnsFalse_IfPrincipalHasOneClaimOfSpecifiedType_But_ClaimDoesNotSatisfyPredicate()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);

            provider.FindClaim(_ClaimA.Type, claims => null);
        }

        [TestMethod]
        public void FindClaim_ReturnsTrue_IfPrincipalHasOneClaimOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimA);            
            
            Assert.IsNotNull(provider.FindClaim(_ClaimA.Type));
        }

        [TestMethod]
        public void FindClaim_ReturnsTrue_IfPrincipalHasManyClaimsOfSpecifiedType()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);
            
            Assert.IsNotNull(provider.FindClaim(_ClaimB_OfType_Int.Type));
        }

        [TestMethod]
        public void FindClaim_ReturnsTrue_IfPrincipalHasManyClaimsOfSpecifiedType_And_AtLeastOneSatisfiesThePredicate()
        {
            var provider = CreateProvider(_ClaimA, _ClaimB_OfType_Int, _ClaimB_OfType_Single);
            
            Assert.IsNotNull(provider.FindClaim(_ClaimB_OfType_Int.Type, c => c.ValueType == _ValueTypeSingle));
        }

        #endregion

        private static ClaimsProvider CreateProvider(params Claim[] claims) =>
            new ClaimsProvider(CreateClaimsPrincipal(claims));

        private static ClaimsPrincipal CreateClaimsPrincipal(params Claim[] claims) =>
            new ClaimsPrincipal(claims.Select(CreateClaimsIdentity));

        private static ClaimsIdentity CreateClaimsIdentity(Claim claim) =>
            new ClaimsIdentity(new [] { claim });
    }
}
