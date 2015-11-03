using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed class ConstraintTest : CompositeConstraintTest
    {
        private Random _randomizer;

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();

            _randomizer = new Random();
        }

        #region [====== Any ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Any_Throws_IfConstraintsIsNull()
        {
            Constraint.Any<object>(null);
        }

        [TestMethod]
        public void Any_ReturnsNullConstraint_IfNoConstraintsAreSpecified()
        {            
            var orConstraint = Constraint.Any(new IConstraint<object>[0]);
            IErrorMessage errorMessage;

            Assert.IsTrue(orConstraint.IsSatisfiedBy(null));
            Assert.IsFalse(orConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);  
        }

        [TestMethod]
        public void Any_ReturnsSatisfiedConstraint_IfOneSatisfiedConstraintIsSpecified()
        {            
            var constraint = NewConstraint(true);
            var orConstraint = Constraint.Any(constraint);
            IErrorMessage errorMessage;

            Assert.IsTrue(orConstraint.IsSatisfiedBy(null));
            Assert.IsFalse(orConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);            
        }

        [TestMethod]
        public void Any_ReturnsUnsatisfiedConstraint_IfOneUnsatisfiedConstraintIsSpecified()
        {
            var constraint = NewConstraint(false);
            var orConstraint = Constraint.Any(constraint).WithErrorMessage(ParentErrorMessage);
            IErrorMessage errorMessage;

            Assert.IsFalse(orConstraint.IsSatisfiedBy(null));
            Assert.IsTrue(orConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void Any_ReturnsSatisfiedConstraint_IfAtLeastOneConstraintIsSatisfied()
        {
            var constraints = CreateConstraintArray();
            var randomIndex = GetRandomIndex(constraints);

            constraints[randomIndex] = NewConstraint(true);

            var orConstraint = Constraint.Any(constraints);
            IErrorMessage errorMessage;

            Assert.IsTrue(orConstraint.IsSatisfiedBy(null));
            Assert.IsFalse(orConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);    
        }

        [TestMethod]
        public void Any_ReturnsUnsatisfiedConstraint_IfNoConstraintIsSatisfied()
        {
            var constraints = CreateConstraintArray(false);            
            var orConstraint = Constraint.Any(constraints).WithErrorMessage(ParentErrorMessage);
            IErrorMessage errorMessage;

            Assert.IsFalse(orConstraint.IsSatisfiedBy(null));
            Assert.IsTrue(orConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("parent", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        #endregion

        #region [====== All ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void All_Throws_IfConstraintsIsNull()
        {
            Constraint.All<object>(null);
        }

        [TestMethod]
        public void All_ReturnsNullConstraint_IfNoConstraintsAreSpecified()
        {
            var andConstraint = Constraint.All(new IConstraint<object>[0]);
            IErrorMessage errorMessage;

            Assert.IsTrue(andConstraint.IsSatisfiedBy(null));
            Assert.IsFalse(andConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void All_ReturnsSatisfiedConstraint_IfOneSatisfiedConstraintIsSpecified()
        {
            var constraint = NewConstraint(true);
            var andConstraint = Constraint.All(constraint);
            IErrorMessage errorMessage;

            Assert.IsTrue(andConstraint.IsSatisfiedBy(null));
            Assert.IsFalse(andConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void All_ReturnsUnsatisfiedConstraint_IfOneUnsatisfiedConstraintIsSpecified()
        {
            var constraint = NewConstraint(false).WithErrorMessage("{child} is not satisfied.");
            var andConstraint = Constraint.All(constraint);
            IErrorMessage errorMessage;

            Assert.IsFalse(andConstraint.IsSatisfiedBy(null));
            Assert.IsTrue(andConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("child", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        [TestMethod]
        public void All_ReturnsSatisfiedConstraint_IfAllConstraintsAreSatisfied()
        {
            var constraints = CreateConstraintArray(true);            
            var andConstraint = Constraint.All(constraints);
            IErrorMessage errorMessage;

            Assert.IsTrue(andConstraint.IsSatisfiedBy(null));
            Assert.IsFalse(andConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNull(errorMessage);
        }

        [TestMethod]
        public void All_ReturnsUnsatisfiedConstraint_IfAtLeastOneConstraintIsNotSatisfied()
        {
            var constraints = CreateConstraintArray(true);
            var randomIndex = GetRandomIndex(constraints);

            constraints[randomIndex] = NewConstraint(false).WithErrorMessage("{child} is not satisfied.");

            var andConstraint = Constraint.All(constraints);
            IErrorMessage errorMessage;

            Assert.IsFalse(andConstraint.IsSatisfiedBy(null));
            Assert.IsTrue(andConstraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.IsNotNull(errorMessage);

            errorMessage.Put("child", "x");

            Assert.AreEqual("x is not satisfied.", errorMessage.ToString());
        }

        #endregion

        private IConstraint<object>[] CreateConstraintArray(bool? isSatisfied = null)
        {            
            var constraints = new IConstraint<object>[_randomizer.Next(2, 10)];

            for (int index = 0; index < constraints.Length; index++)
            {
                constraints[index] = NewConstraint(isSatisfied ?? MayBeSatisfied());
            }
            return constraints;
        }

        private bool MayBeSatisfied()
        {
            return _randomizer.NextDouble() <= 0.5;
        }

        private int GetRandomIndex(IConstraint<object>[] constraints)
        {
            return _randomizer.Next(0, constraints.Length - 1);
        }
    }
}
