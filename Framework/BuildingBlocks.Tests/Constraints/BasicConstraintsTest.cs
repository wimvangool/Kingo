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
        public void ValidateInstance_ReturnsNoErrors_IfNoConstraintsAreSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatInstance();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateInstance_ReturnsNoErrors_IfConstraintIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatInstance().Satisfies(m => true);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateInstance_ReturnsExpectedError_IfConstraintIsNotSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatInstance().Satisfies(m => false, RandomErrorMessage);

            validator.Validate(message).AssertError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateMember_ReturnsNoErrors_IfConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => true);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateMember_ReturnsExpectedError_IfConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => false, RandomErrorMessage);

            validator.Validate(message).AssertMemberError(RandomErrorMessage);
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

        #region [====== HaltOnFirstError ======]

        [TestMethod]
        public void Validate_ReturnsAllErrors_IfMultipleConstraintsFail_And_HaltOnFirstErrorIsFalse()
        {
            var message = new ValidatedMessage<int>(0, 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);
            validator.VerifyThat(m => m.Other).IsEqualTo(m => m.Member);

            validator.Validate(message)
                .AssertErrorCountIs(2)
                .AssertMemberError("Member (0) must be equal to '1'.", "Member")
                .AssertMemberError("Other (1) must be equal to '0'.", "Other");
        }

        [TestMethod]
        public void Validate_ReturnsOnlyFirstError_IfMultipleConstraintsFail_And_HaltOnFirstErrorIsTrue()
        {
            var message = new ValidatedMessage<int>(0, 1);
            var validator = message.CreateConstraintValidator(true);

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);
            validator.VerifyThat(m => m.Other).IsEqualTo(m => m.Member);

            validator.Validate(message)
                .AssertErrorCountIs(1)
                .AssertMemberError("Member (0) must be equal to '1'.", "Member");                
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

            validator.Validate(message).AssertMemberError("Member (0) must be greater than '0'.");
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfSecondConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_SecondConstraintFails()
        {
            var message = new ValidatedMessage<int>(10);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).IsSmallerThan(10);

            validator.Validate(message).AssertMemberError("Member (10) must be smaller than '10'.");
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfFirstChildConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_FirstChildConstraintFails()
        {
            var message = new ValidatedMessage<int[]>(new [] { 0 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).ElementAt(0).IsSmallerThan(10);

            validator.Validate(message).AssertMemberError("Member[0] (0) must be greater than '0'.", "Member[0]");
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfSecondChildConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_SecondChildConstraintFails()
        {
            var message = new ValidatedMessage<int[]>(new [] { 10 } );
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).ElementAt(0).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).ElementAt(0).IsSmallerThan(10);

            validator.Validate(message).AssertMemberError("Member[0] (10) must be smaller than '10'.", "Member[0]");
        }

        #endregion

        #region [====== Constraints On Fields Or Properties ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintOnPropertyFails()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And(member => member.Length).IsEqualTo(0);

            validator.Validate(message).AssertMemberError("Member.Length (10) must be equal to '0'.");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintOnPropertyOfPropertyFails()
        {
            var message = new ValidatedMessage<ValidatedMessage<string>>(new ValidatedMessage<string>("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull()
                .And(member => member.Member).IsNotNull()
                .And(member => member.Length).IsEqualTo(0);

            validator.Validate(message).AssertMemberError("Member.Member.Length (10) must be equal to '0'.");
        }

        #endregion

        #region [====== Default Member ======]

        private const string _MemberErrorMessage = "| {member.Key} | {member.FullName} | {member.Name} | {member.Type} | {member.Value} |";

        [TestMethod]
        public void IsNotSatisfiedBy_DefaultsToDefaultMember_IfMemberIsNotSpecifiedAsArgumentForErrorMessage_And_ValueIsNull()
        {
            IErrorMessage errorMessage;

            var constraint = new IsEqualToConstraint<string>("Some value").WithErrorMessage(_MemberErrorMessage);
            
            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.AreEqual("| Value | Value | Value | System.String | <null> |", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_DefaultsToDefaultMember_IfMemberIsNotSpecifiedAsArgumentForErrorMessage_And_ValueIsNotNull()
        {
            IErrorMessage errorMessage;

            var constraint = new IsEqualToConstraint<int>(0).WithErrorMessage(_MemberErrorMessage);

            Assert.IsTrue(constraint.IsNotSatisfiedBy(1, out errorMessage));
            Assert.AreEqual("| Value | Value | Value | System.Int32 | 1 |", errorMessage.ToString());
        }

        #endregion
    }
}
