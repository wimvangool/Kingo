using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    [TestClass]
    public sealed class MemberConstraintTest : ConstraintTest
    {       
        #region [====== Basics ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {            
            var validator = new ConstraintValidator();

            validator.Validate().AssertNoErrors();            
        }

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).Satisfies(new TrueConstraint<object>());

            validator.Validate().AssertNoErrors();            
        }        

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyThat_Throws_IfExpressionIsArrayIndexer()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member[1]);
        }

        #endregion

        #region [====== And ======]

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void And_Throws_IfArgumentIsNull()
        //{
        //    var message = new ValidatedMessage<object>(new object());
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member).IsNotNull(ErrorMessage).And(null); ;
        //}

        //[TestMethod]
        //public void And_ReturnsNoErrors_IfChildValidationSucceeds()
        //{
        //    var message = new ValidatedMessage<object>("Some value");
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member)
        //        .IsNotNull(ErrorMessage)
        //        .IsInstanceOf<string>(ErrorMessage)
        //        .And(value => validator.VerifyThat(() => value.Length).IsEqualTo(10, ErrorMessage));

        //    validator.Validate().AssertNoErrors();
        //}

        //[TestMethod]
        //public void And_ReturnsExpectedError_IfChildValidationFails()
        //{
        //    var message = new ValidatedMessage<object>("Some value");
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member)
        //        .IsNotNull(ErrorMessage)
        //        .IsInstanceOf<string>(ErrorMessage)
        //        .And(value => validator.VerifyThat(() => value.Length).IsNotEqualTo(10, ErrorMessage));

        //    validator.Validate().AssertOneError(ErrorMessage, "Member.Length");
        //}

        //[TestMethod]
        //public void And_ReturnsExpectedError_IfChildOfChildValidationFails()
        //{
        //    var message = new ValidatedMessage<ValidatedMessage<object>>(new ValidatedMessage<object>("Some value"));
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member).IsNotNull(ErrorMessage).And(innerMessage =>            
        //        validator.VerifyThat(() => innerMessage.Member).IsInstanceOf<string>().And(value =>                
        //            validator.VerifyThat(() => value.Length).IsNotEqualTo(10, ErrorMessage)
        //        )
        //    );

        //    validator.Validate().AssertOneError(ErrorMessage, "Member.Member.Length");
        //}

        #endregion

        #region [====== IsNotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfObjectIsNotNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNull(RandomErrorMessage);

            validator.Validate().AssertNoErrors();                     
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfObjectIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNull(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsDefaultError_IfObjectIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotNull();

            validator.Validate().AssertOneError("Member (<null>) must refer to an instance of an object.");
        }

        #endregion     
   
        #region [====== IsNull ======]

        [TestMethod]
        public void ValidateIsNull_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNull(RandomErrorMessage);

            validator.Validate().AssertNoErrors();            
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsExpectedError_IfObjectIsNotNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNull(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsDefaultError_IfObjectIsNotNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNull();

            validator.Validate().AssertOneError("Member (System.Object) must be null.");
        }

        #endregion

        #region [====== IsNotSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfBothObjectsAreNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(null);

            validator.Validate().AssertOneError("Member (<null>) must not refer to the same instance as '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(member, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(member);

            validator.Validate().AssertOneError("Member (System.Object) must not refer to the same instance as 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsNoErrors_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(new object(), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(member, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsExpectedError_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(new object(), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsDefaultError_IfObjectsAreNotSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(new object());

            validator.Validate().AssertOneError("Member (System.Object) must refer to the same instance as 'System.Object'.");
        }

        #endregion

        #region [====== IsNotInstanceOf =======]

        [TestMethod]        
        public void ValidateIsNotInstanceOf_ReturnsNoErrors_IfMemberIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf(typeof(object), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]       
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf<object>(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsExpectedError_IfObjectIsNotInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf<object>(RandomErrorMessage);                

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsDefaultError_IfObjectIsNotInstanceOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf<object>();                

            validator.Validate().AssertOneError("Member of type 'System.String' must not be an instance of type 'System.Object'.");
        } 

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf<int>(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsExpectedError_IfObjectIsNotInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsNotInstanceOf(typeof(object), RandomErrorMessage)
                .IsNotInstanceOf(typeof(string), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsNoError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsInstanceOf =======]

        [TestMethod]        
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf<object>(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf<int>(RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf<int>();

            validator.Validate().AssertOneError("Member of type 'System.String' must be an instance of type 'System.Int32'.");
        } 

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfObjectIsInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<object>(RandomErrorMessage)
                .IsInstanceOf<string>(RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]        
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf(typeof(object), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf(typeof(int));

            validator.Validate().AssertOneError("Member of type 'System.String' must be an instance of type 'System.Int32'.");
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsNoErrors_IfObjectIsInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf(typeof(object), RandomErrorMessage)
                .IsInstanceOf(typeof(string), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }               

        #endregion           

        #region [====== IsNotEqualTo ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreBothNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null as object, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreBothNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null as object);

            validator.Validate().AssertOneError("Member (<null>) must not be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member);

            validator.Validate().AssertOneError("Member (System.Object) must not be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreEqual_And_NoErrorMessageIsSpecified()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(memberValue.ToString());

            validator.Validate().AssertOneError(string.Format("Member ({0}) must not be equal to '{0}'.", memberValue));
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null as object, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some other value", RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreBothNull_And_ComparerIsNull()
        {                   
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreSameInstance_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member, comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(memberValue.ToString(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfStringsAreNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some other value", comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(Guid.NewGuid(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null, comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsExpectedError_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsNoErrors_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(member + 1, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion      

        #region [====== IsEqualTo ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreBothNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfMemberIsNullAndTheOtherIsNot_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object());

            validator.Validate().AssertOneError("Member (<null>) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfOtherIsNullAndTheMemberIsNot_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null);

            validator.Validate().AssertOneError("Member (System.Object) must be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfObjectsAreNotNullAndNotEqual_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object());

            validator.Validate().AssertOneError("Member (System.Object) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some other value", RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreBothNull_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreSameInstance_And_ComparerIsNull()
        {            
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(member, null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreEqual_And_ComparerIsNull()
        {            
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(memberValue.ToString(), null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfStringsAreNotEqual_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>("Some value");
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some other value", null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(Guid.NewGuid(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsNoErrors_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsExpectedError_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(member + 1, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThan ======]

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member + 1, null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, null);

            validator.Validate().AssertOneError(string.Format("Member ({0}) must be smaller than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member - 1, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member + 1, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member - 1, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member + 1, null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member, null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsDefaultError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, null);

            validator.Validate().AssertOneError(string.Format("Member ({0}) must be smaller than or equal to '{1}'.", member, member - 1));
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member + 1, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsGreaterThan ======]

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member + 1, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, null, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, null);

            validator.Validate().AssertOneError(string.Format("Member ({0}) must be greater than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member - 1, null, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member + 1, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member - 1, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, null);

            validator.Validate().AssertOneError(string.Format("Member ({0}) must be greater than or equal to '{1}'.", member, member + 1));
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member, comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member - 1, comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member - 1, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsNotInRange ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(null, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsNotInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsNotInRange(new object(), new object(), RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(member, member - 1, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsExpectedError_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsDefaultError_IfMemberIsInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive);

            validator.Validate().AssertOneError(string.Format("Member ({0}) must not be within the following range: [0, 1000>.", member));
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsNoErrors_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(1000, 2000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsInRange ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsInRange_Throws_IfRangeIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(null, RandomErrorMessage);          
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInRange(new object(), new object(), RandomErrorMessage);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(member, member - 1, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsNoErrors_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(0, 1000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsExpectedError_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate().AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsDefaultError_IfMemberIsNotInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new ConstraintValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive);

            validator.Validate().AssertOneError(string.Format("Member ({0}) must be within the following range: [1000, 2000>.", member));
        }

        #endregion
    }
}
