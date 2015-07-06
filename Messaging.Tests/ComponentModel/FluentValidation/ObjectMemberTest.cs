using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.FluentValidation
{
    [TestClass]
    public sealed class ObjectMemberTest
    {       
        private string _errorMessage;

        [TestInitialize]
        public void Setup()
        {
            _errorMessage = Guid.NewGuid().ToString("N");
        }

        #region [====== Basics ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {            
            var validator = new FluentValidator();

            validator.Validate().AssertNoErrors();            
        }

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).Satisfies(value => true, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintIsNotSatisfied()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).Satisfies(value => false, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void Validate_ReturnsExpectedErrorWithOneArgument_IfConstraintIsNotSatisfied()
        {
            var errorMessageFormat = _errorMessage + @"_{0}";
            var arg0 = Guid.NewGuid();

            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).Satisfies(value => false, new FormattedString(errorMessageFormat, arg0));

            validator.Validate().AssertOneError(string.Format(errorMessageFormat, arg0));
        }

        [TestMethod]
        public void Validate_ReturnsExpectedErrorWithTwoArguments_IfConstraintIsNotSatisfied()
        {
            var errorMessageFormat = _errorMessage + @"_{0}_{1}";
            var arg0 = Guid.NewGuid();
            var arg1 = Guid.NewGuid();

            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).Satisfies(value => false, new FormattedString(errorMessageFormat, arg0, arg1));

            validator.Validate().AssertOneError(string.Format(errorMessageFormat, arg0, arg1));
        }

        [TestMethod]
        public void Validate_ReturnsExpectedErrorWithThreeArguments_IfConstraintIsNotSatisfied()
        {
            var errorMessageFormat = _errorMessage + @"_{0}_{1}_{2}";
            var arg0 = Guid.NewGuid();
            var arg1 = Guid.NewGuid();
            var arg2 = Guid.NewGuid();

            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).Satisfies(value => false, new FormattedString(errorMessageFormat, arg0, arg1, arg2));

            validator.Validate().AssertOneError(string.Format(errorMessageFormat, arg0, arg1, arg2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void VerifyThat_Throws_IfExpressionIsArrayIndexer()
        {
            var message = new ValidatedMessage<int[]>(new[] { 0, 1, 2, 3 });
            var validator = new FluentValidator();

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

        //    validator.VerifyThat(() => message.Member).IsNotNull(_errorMessage).And(null); ;
        //}

        //[TestMethod]
        //public void And_ReturnsNoErrors_IfChildValidationSucceeds()
        //{
        //    var message = new ValidatedMessage<object>("Some value");
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member)
        //        .IsNotNull(_errorMessage)
        //        .IsInstanceOf<string>(_errorMessage)
        //        .And(value => validator.VerifyThat(() => value.Length).IsEqualTo(10, _errorMessage));

        //    validator.Validate().AssertNoErrors();
        //}

        //[TestMethod]
        //public void And_ReturnsExpectedError_IfChildValidationFails()
        //{
        //    var message = new ValidatedMessage<object>("Some value");
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member)
        //        .IsNotNull(_errorMessage)
        //        .IsInstanceOf<string>(_errorMessage)
        //        .And(value => validator.VerifyThat(() => value.Length).IsNotEqualTo(10, _errorMessage));

        //    validator.Validate().AssertOneError(_errorMessage, "Member.Length");
        //}

        //[TestMethod]
        //public void And_ReturnsExpectedError_IfChildOfChildValidationFails()
        //{
        //    var message = new ValidatedMessage<ValidatedMessage<object>>(new ValidatedMessage<object>("Some value"));
        //    var validator = new FluentValidator();

        //    validator.VerifyThat(() => message.Member).IsNotNull(_errorMessage).And(innerMessage =>            
        //        validator.VerifyThat(() => innerMessage.Member).IsInstanceOf<string>().And(value =>                
        //            validator.VerifyThat(() => value.Length).IsNotEqualTo(10, _errorMessage)
        //        )
        //    );

        //    validator.Validate().AssertOneError(_errorMessage, "Member.Member.Length");
        //}

        #endregion

        #region [====== IsNotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfObjectIsNotNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNull(_errorMessage);

            validator.Validate().AssertNoErrors();                     
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfObjectIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotNull(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        #endregion     
   
        #region [====== IsNull ======]

        [TestMethod]
        public void ValidateIsNull_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNull(_errorMessage);

            validator.Validate().AssertNoErrors();            
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsExpectedError_IfObjectIsNotNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNull(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);            
        }

        #endregion

        #region [====== IsNotInstanceOf =======]

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsExpectedError_IfObjectIsNotInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsNotInstanceOf<object>(_errorMessage)
                .IsNotInstanceOf<string>(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf<object>(_errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf<int>(_errorMessage);

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsExpectedError_IfObjectIsNotInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsNotInstanceOf(typeof(object), _errorMessage)
                .IsNotInstanceOf(typeof(string), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsNoError_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf(typeof(object), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsNoError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotInstanceOf(typeof(int), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsInstanceOf =======]

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfObjectIsInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<object>(_errorMessage)
                .IsInstanceOf<string>(_errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf<object>(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf<int>(_errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }       

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsNoErrors_IfObjectIsInstanceOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf(typeof(object), _errorMessage)
                .IsInstanceOf(typeof(string), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf(typeof(object), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsInstanceOf(typeof(int), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion           

        #region [====== IsNotEqualTo ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreBothNull()
        {
            object member = null;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(memberValue.ToString(), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null as object, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some other value", _errorMessage);

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreBothNull_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            object member = null;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreSameInstance_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(member, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(memberValue.ToString(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null as object, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfStringsAreNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo("Some other value", comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(Guid.NewGuid(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotEqualTo(null, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsExpectedError_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsNotEqualTo(member, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsNoErrors_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsNotEqualTo(member + 1, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsNotSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(null, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(member, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsNoErrors_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsNotSameInstanceAs(new object(), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        #endregion

        #region [====== IsSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(null, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(member, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsExpectedError_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSameInstanceAs(new object(), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsEqualTo ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreBothNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(member, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(memberValue.ToString(), _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some other value", _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }        

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreBothNull_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreSameInstance_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(member, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(memberValue.ToString(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfStringsAreNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value");
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo("Some other value", comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(Guid.NewGuid(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsEqualTo(null, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }        

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsNoErrors_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsEqualTo(member, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsExpectedError_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsEqualTo(member + 1, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsSmallerThan ======]

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThan(member + 1, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThan(member, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThan(member - 1, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThan(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThan(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThan(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThan(member + 1, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThan(member, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThan(member - 1, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThanOrEqualTo(member + 1, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThanOrEqualTo(member, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThanOrEqualTo(member - 1, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThanOrEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThanOrEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsSmallerThanOrEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThanOrEqualTo(member + 1, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThanOrEqualTo(member, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsSmallerThanOrEqualTo(member - 1, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion

        #region [====== IsGreaterThan ======]

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThan(member + 1, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThan(member, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThan(member - 1, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThan(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThan(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThan(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThan(member + 1, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThan(member, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThan(member - 1, _errorMessage);

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
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThanOrEqualTo(member + 1, comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThanOrEqualTo(member, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThanOrEqualTo(member - 1, comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThanOrEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThanOrEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member).IsGreaterThanOrEqualTo(new object(), comparer, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThanOrEqualTo(member + 1, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThanOrEqualTo(member, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<long>(_errorMessage)
                .IsGreaterThanOrEqualTo(member - 1, _errorMessage);

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
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsNotInRange(null, _errorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsNotInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsNotInRange(new object(), new object(), _errorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsNotInRange(member, member - 1, _errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsExpectedError_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsNoErrors_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsNotInRange(1000, 2000, RangeOptions.RightExclusive, _errorMessage);

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
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsInRange(null, _errorMessage);          
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInRange(new object(), new object(), _errorMessage);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsInRange(member, member - 1, _errorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsNoErrors_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsInRange(0, 1000, RangeOptions.RightExclusive, _errorMessage);

            validator.Validate().AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsExpectedError_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = new FluentValidator();

            validator.VerifyThat(() => message.Member)
                .IsInstanceOf<int>(_errorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive, _errorMessage);

            validator.Validate().AssertOneError(_errorMessage);
        }

        #endregion
    }
}
