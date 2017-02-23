using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Constraints
{
    [TestClass]
    public sealed partial class BasicConstraintsTest : ConstraintTestBase
    {        
        #region [====== Basics ======]
     
        [TestMethod]
        public void Validate_ReturnsNoErrors_IfNoConstraintsAreSpecified()
        {
            var validator = new ConstraintMessageValidator<EmptyMessage>();

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

            validator.Validate(message).AssertInstanceError(RandomErrorMessage);
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
                .AssertErrorCountIs(3)
                .AssertInstanceError("Member (0) must be equal to '1'.")
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
                .AssertErrorCountIs(2)
                .AssertInstanceError("Member (0) must be equal to '1'.")
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
            var message = new ValidatedMessage<int[]>(new[] { 0 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasItem<int>(0).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).HasItem<int>(0).IsSmallerThan(10);

            validator.Validate(message).AssertMemberError("Member[0] (0) must be greater than '0'.", "Member[0]");            
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfSecondChildConstraint_IfMultipleConstraintsPerMemberAreSpecified_And_SecondChildConstraintFails()
        {
            var message = new ValidatedMessage<int[]>(new[] { 10 });
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).HasItem<int>(0).IsGreaterThan(0);
            validator.VerifyThat(m => m.Member).HasItem<int>(0).IsSmallerThan(10);

            validator.Validate(message).AssertMemberError("Member[0] (10) must be smaller than '10'.", "Member[0]");            
        }

        #endregion

        #region [====== Mixed Constraints on Self and Members ======]

        [TestMethod]
        public void Validate_ValidatesInstanceConstraints_IfMemberConstraintsPass()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThatInstance().IsNull(RandomErrorMessage);
            validator.VerifyThat(m => m.Member).IsNotNull();

            validator.Validate(message).AssertInstanceError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_DoesNotValidateInstanceConstraints_IfMemberConstraintsFail()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();
            
            validator.VerifyThatInstance().IsNull();
            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message)
                .AssertErrorCountIs(2)
                .AssertInstanceError(RandomErrorMessage)
                .AssertMemberError(RandomErrorMessage);          
        }

        #endregion

        #region [====== Constraints On Fields Or Properties ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintOnPropertyFails()
        {
            var message = new ValidatedMessage<string>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And((m, member) => member.Length).IsEqualTo(0);

            validator.Validate(message).AssertMemberError("Member.Length (10) must be equal to '0'.");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintOnPropertyOfPropertyFails()
        {
            var message = new ValidatedMessage<ValidatedMessage<string>>(new ValidatedMessage<string>("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull()
                .And((m, member) => member.Member).IsNotNull()
                .And((m, member) => member.Length).IsEqualTo(0);

            validator.Validate(message).AssertMemberError("Member.Member.Length (10) must be equal to '0'.");
        }

        #endregion

        #region [====== Default Member ======]

        private const string _MemberErrorMessage = "| {member.FullName} | {member.Name} | {member.Type} | {member.Value} |";

        [TestMethod]
        public void IsNotSatisfiedBy_DefaultsToDefaultMember_IfMemberIsNotSpecifiedAsArgumentForErrorMessage_And_ValueIsNull()
        {
            IErrorMessageBuilder errorMessage;

            var constraint = new IsEqualToConstraint<string>("Some value").WithErrorMessage(_MemberErrorMessage);
            
            Assert.IsTrue(constraint.IsNotSatisfiedBy(null, out errorMessage));
            Assert.AreEqual("| Value | Value | System.String | <null> |", errorMessage.ToString());
        }

        [TestMethod]
        public void IsNotSatisfiedBy_DefaultsToDefaultMember_IfMemberIsNotSpecifiedAsArgumentForErrorMessage_And_ValueIsNotNull()
        {
            IErrorMessageBuilder errorMessage;

            var constraint = new IsEqualToConstraint<int>(0).WithErrorMessage(_MemberErrorMessage);

            Assert.IsTrue(constraint.IsNotSatisfiedBy(1, out errorMessage));
            Assert.AreEqual("| Value | Value | System.Int32 | 1 |", errorMessage.ToString());
        }

        #endregion        
    }
}
