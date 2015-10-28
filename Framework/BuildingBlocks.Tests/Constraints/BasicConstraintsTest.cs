using System;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Constraints
{
    [TestClass]
    public sealed partial class BasicConstraintsTest : ConstraintTestBase
    {       
        #region [====== Basics ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfNoConstraintsAreSpecified()
        {            
            var validator = new ConstraintValidator<EmptyMessage>();

            validator.Validate(new EmptyMessage()).AssertNoErrors();            
        }

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => true);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => false, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }           

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyThat_Throws_IfExpressionIsArrayIndexer()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member[1]);
        }

        #endregion                                                                

        #region [====== Multiple Constraints Per Member ======]

        [TestMethod]
        public void Validate_ReturnsErrorOfFirstConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_FirstConstraintFails()
        {
            var message = new ValidatedMessage<int>(0);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).IsSmallerThan(10);

            validator.Validate(message).AssertOneError("Member (0) must be greater than '0'.");
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfSecondConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_SecondConstraintFails()
        {
            var message = new ValidatedMessage<int>(10);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).IsSmallerThan(10);

            validator.Validate(message).AssertOneError("Member (10) must be smaller than '10'.");
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfFirstChildConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_FirstChildConstraintFails()
        {
            var message = new ValidatedMessage<int[]>(new [] { 0 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).ElementAt(0).IsSmallerThan(10);

            validator.Validate(message).AssertOneError("Member[0] (0) must be greater than '0'.", "Member[0]");
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfSecondChildConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_SecondChildConstraintFails()
        {
            var message = new ValidatedMessage<int[]>(new [] { 10 } );
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).ElementAt(0).IsSmallerThan(10);

            validator.Validate(message).AssertOneError("Member[0] (10) must be smaller than '10'.", "Member[0]");
        }

        #endregion

        #region [====== Constraints On Fields Or Properties ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintOnPropertyFails()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And(member => member.Length).IsEqualTo(0);

            validator.Validate(message).AssertOneError("Member.Length (10) must be equal to '0'.");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintOnPropertyOfPropertyFails()
        {
            var message = new ValidatedMessage<ValidatedMessage<string>>(new ValidatedMessage<string>("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull()
                .And(member => member.Member).IsNotNull()
                .And(member => member.Length).IsEqualTo(0);

            validator.Validate(message).AssertOneError("Member.Member.Length (10) must be equal to '0'.");
        }

        #endregion
    }
}
