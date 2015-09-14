using System;
using System.Collections.Generic;
using Kingo.BuildingBlocks.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    [TestClass]
    public sealed class MemberConstraintTest : ConstraintTest
    {       
        #region [====== Basics ======]        

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfNoConstraintsAreSpecified()
        {            
            var validator = new ConstraintValidator<EmptyMessage>();

            validator.Validate(new EmptyMessage()).AssertNoErrors();            
        }

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied_1()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => true);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintIsNotSatisfied_1()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => false, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied_2()
        {
            var message = new ValidatedMessage<int>(Clock.Current.LocalDateAndTime().Millisecond);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => value >= 0, value => value.ToString()).IsInt32();

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintIsNotSatisfied_2()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(value => false, value => value.ToString(), RandomErrorMessage).IsInt32();

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied_3()
        {
            var message = new ValidatedMessage<string>(Clock.Current.LocalDateAndTime().Millisecond.ToString());
            var validator = message.CreateConstraintValidator();            

            validator.VerifyThat(m => m.Member).Satisfies(delegate (string value, ValidatedMessage<string> m, out int result)
            {
                return int.TryParse(value, out result);
            }).IsGreaterThanOrEqualTo(0);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfConstraintIsNotSatisfied_3()
        {
            var message = new ValidatedMessage<string>(Guid.NewGuid().ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(delegate(string value, ValidatedMessage<string> m, out int result)
            {
                return int.TryParse(value, out result);
            }, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfConstraintIsSatisfied_4()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).Satisfies(new TrueConstraint<ValidatedMessage<object>, object>());

            validator.Validate(message).AssertNoErrors();            
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

        #region [====== And ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void And_Throws_IfArgumentIsNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And(null);
        }

        [TestMethod]
        public void And_ReturnsNoErrors_IfChildValidationSucceeds()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().IsInstanceOf<string>().And(v =>
            {
                v.VerifyThat(value => value.Length).IsEqualTo(10);
            });

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfChildValidationFails()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().IsInstanceOf<string>().And(v =>
            {
                v.VerifyThat(value => value.Length).IsNotEqualTo(10, RandomErrorMessage);
            });

            validator.Validate(message).AssertOneError(RandomErrorMessage, "Member.Length");
        }

        [TestMethod]
        public void And_ReturnsExpectedError_IfChildOfChildValidationFails()
        {
            var message = new ValidatedMessage<ValidatedMessage<object>>(new ValidatedMessage<object>("Some value"));
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull().And(v1 =>
            {
                v1.VerifyThat(childMessage => childMessage.Member).IsInstanceOf<string>().And(v2 =>
                {
                    v2.VerifyThat(value => value.Length).IsNotEqualTo(10, RandomErrorMessage);
                });
            });

            validator.Validate(message).AssertOneError(RandomErrorMessage, "Member.Member.Length");
        }

        #endregion

        #region [====== IsNotNull ======]

        [TestMethod]
        public void ValidateIsNotNull_ReturnsNoErrors_IfObjectIsNotNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();                    
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsExpectedError_IfObjectIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNotNull_ReturnsDefaultError_IfObjectIsNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotNull();

            validator.Validate(message).AssertOneError("Member (<null>) must refer to an instance of an object.");
        }        

        #endregion     
   
        #region [====== IsNull ======]

        [TestMethod]
        public void ValidateIsNull_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();            
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsExpectedError_IfObjectIsNotNull()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);            
        }

        [TestMethod]
        public void ValidateIsNull_ReturnsDefaultError_IfObjectIsNotNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNull();

            validator.Validate(message).AssertOneError("Member (System.Object) must be null.");
        }        

        #endregion

        #region [====== IsNotSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(null as object, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfBothObjectsAreNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(null as object);

            validator.Validate(message).AssertOneError("Member (<null>) must not refer to the same instance as '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(member, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(member);

            validator.Validate(message).AssertOneError("Member (System.Object) must not refer to the same instance as 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsNoErrors_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }        

        #endregion

        #region [====== IsNotSameInstanceAs (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsNotSameInstanceAs_Throws_IfOtherFactoryIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(null);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfBothObjectsAreNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfBothObjectsAreNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => null);

            validator.Validate(message).AssertOneError("Member (<null>) must not refer to the same instance as '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsExpectedError_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => m.Other);

            validator.Validate(message).AssertOneError("Member (System.Object) must not refer to the same instance as 'Other (System.Object)'.");
        }

        [TestMethod]
        public void ValidateIsNotSameInstanceAs_ReturnsNoErrors_IfObjectsAreNotSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotSameInstanceAs(m => new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }    

        #endregion

        #region [====== IsSameInstanceAs ======]

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfBothObjectsAreNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(null as object, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsExpectedError_IfObjectsAreNotSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(new object(), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsDefaultError_IfObjectsAreNotSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(new object());

            validator.Validate(message).AssertOneError("Member (System.Object) must refer to the same instance as 'System.Object'.");
        }               

        #endregion

        #region [====== IsSameInstanceAs (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsSameInstanceAs_Throws_IfOtherFactoryIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(null);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfBothObjectsAreNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsNoErrors_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsExpectedError_IfObjectsAreNotSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => new object(), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSameInstanceAs_ReturnsDefaultError_IfObjectsAreNotSameInstance_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSameInstanceAs(m => m.Other);

            validator.Validate(message).AssertOneError("Member (System.Object) must refer to the same instance as 'Other (System.Object)'.");
        }

        #endregion

        #region [====== IsNotInstanceOf =======]

        [TestMethod]        
        public void ValidateIsNotInstanceOf_ReturnsNoErrors_IfMemberIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(typeof(object), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]       
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<object>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<object>(RandomErrorMessage);                

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<object>();                

            validator.Validate(message).AssertOneError("Member of type 'System.Object' must not be an instance of type 'System.Object'.");
        } 

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf<int>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsNotInstanceOf(typeof(object), RandomErrorMessage)
                .IsNotInstanceOf(typeof(string), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void ValidateIsNotInstanceOf_ReturnsNoError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }        

        #endregion

        #region [====== IsNotInstanceOf (Indirect) =======]

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsNoErrors_IfObjectIsNotOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(int)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(m => m.ExpectedMemberType);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotInstanceOfOther_ReturnsExpectedError_IfObjectIsOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(string)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInstanceOf(m => m.ExpectedMemberType, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsInstanceOf =======]

        [TestMethod]        
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfMemberIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<object>(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<int>(RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf<int>();

            validator.Validate(message).AssertOneError("Member of type 'System.Object' must be an instance of type 'System.Int32'.");
        } 

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfObjectIsOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<object>(RandomErrorMessage)
                .IsInstanceOf<string>(RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]        
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(object), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(int), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsDefaultError_IfObjectIsNotOfSpecifiedType_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(typeof(int));

            validator.Validate(message).AssertOneError("Member of type 'System.Object' must be an instance of type 'System.Int32'.");
        }

        [TestMethod]
        public void ValidateIsInstanceOf_ReturnsNoErrors_IfObjectIsOfSpecifiedType()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf(typeof(object), RandomErrorMessage)
                .IsInstanceOf(typeof(string), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsInstanceOf (Indirect) =======]

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsExpectedError_IfObjectIsNotOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(int)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(m => m.ExpectedMemberType, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInstanceOfOther_ReturnsNoErrors_IfObjectIsOfSpecifiedType_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value")
            {
                ExpectedMemberType = typeof(string)
            };
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInstanceOf(m => m.ExpectedMemberType);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotEqualTo ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreBothNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreBothNull_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object);

            validator.Validate(message).AssertOneError("Member (<null>) must not be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(member);

            validator.Validate(message).AssertOneError("Member (System.Object) must not be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreEqual_And_NoErrorMessageIsSpecified()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString());

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must not be equal to '{0}'.", memberValue));
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some other value", RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreBothNull_And_ComparerIsNull()
        {                   
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreSameInstance_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(member, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfStringsAreNotEqual_And_ComparerIsNull()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo("Some other value", comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(Guid.NewGuid(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }        

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsExpectedError_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsNoErrors_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion      

        #region [====== IsNotEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreBothNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreBothNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other);

            validator.Validate(message).AssertOneError("Member (<null>) must not be equal to 'Other (<null>)'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreSameInstance_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other);

            validator.Validate(message).AssertOneError("Member (System.Object) must not be equal to 'Other (System.Object)'.");
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsExpectedError_IfObjectsAreEqual_Indirect()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsDefaultError_IfObjectsAreEqual_And_NoErrorMessageIsSpecified_Indirect()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString(), memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must not be equal to 'Other ({0})'.", memberValue));
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualTo_ReturnsNoErrors_IfStringsAreNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreBothNull_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreSameInstance_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsExpectedError_IfObjectsAreEqual_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString(), memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToByComparer_ReturnsNoErrors_IfStringsAreNotEqual_And_ComparerIsNull_Indirect()
        {
            IEqualityComparer<object> comparer = null;
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsTrue_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid(), Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsFalse_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsExpectedError_IfValuesAreEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotEqualToOther_ReturnsNoErrors_IfValuesAreNotEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsNotEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion      

        #region [====== IsEqualTo ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreBothNull()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreSameInstance()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreEqual()
        {
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(memberValue.ToString(), RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfMemberIsNullAndTheOtherIsNot_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object());

            validator.Validate(message).AssertOneError("Member (<null>) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfOtherIsNullAndTheMemberIsNot_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object);

            validator.Validate(message).AssertOneError("Member (System.Object) must be equal to '<null>'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfObjectsAreNotNullAndNotEqual_And_NoErrorMessageIsSpecified()
        {
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object());

            validator.Validate(message).AssertOneError("Member (System.Object) must be equal to 'System.Object'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfStringsAreNotEqual()
        {
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some other value", RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreBothNull_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreSameInstance_And_ComparerIsNull()
        {            
            var member = new object();
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(member, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreEqual_And_ComparerIsNull()
        {            
            var memberValue = Guid.NewGuid();
            var message = new ValidatedMessage<object>(memberValue.ToString());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(memberValue.ToString(), null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(new object(), null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfStringsAreNotEqual_And_ComparerIsNull()
        {            
            var message = new ValidatedMessage<object>("Some value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo("Some other value", null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsTrue()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(Guid.NewGuid(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsFalse()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(null as object, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }        

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsNoErrors_IfValuesAreEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsExpectedError_IfValuesAreNotEqual()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreBothNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreSameInstance_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsNoErrors_IfObjectsAreEqual_Indirect()
        {
            var member = Guid.NewGuid();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfMemberIsNullAndTheOtherIsNot_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);

            validator.Validate(message).AssertOneError("Member (<null>) must be equal to 'Other (System.Object)'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfOtherIsNullAndTheMemberIsNot_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);

            validator.Validate(message).AssertOneError("Member (System.Object) must be equal to 'Other (<null>)'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsDefaultError_IfObjectsAreNotNullAndNotEqual_And_NoErrorMessageIsSpecified_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other);

            validator.Validate(message).AssertOneError("Member (System.Object) must be equal to 'Other (System.Object)'.");
        }

        [TestMethod]
        public void ValidateIsEqualTo_ReturnsExpectedError_IfStringsAreNotEqual_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreBothNull_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreSameInstance_And_ComparerIsNull_Indirect()
        {
            var member = new object();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfObjectsAreEqual_And_ComparerIsNull_Indirect()
        {
            var member = Guid.NewGuid();
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfMemberIsNullAndTheOtherIsNot_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(null, new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfOtherIsNullAndTheMemberIsNot_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfObjectsAreNotNullAndNotEqual_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfStringsAreNotEqual_And_ComparerIsNull_Indirect()
        {
            var message = new ValidatedMessage<object>("Some value", "Some other value");
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsNoErrors_IfComparerReturnsTrue_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(true);
            var message = new ValidatedMessage<object>(Guid.NewGuid(), Guid.NewGuid());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToByComparer_ReturnsExpectedError_IfComparerReturnsFalse_Indirect()
        {
            var comparer = new EqualityComparerStub<object>(false);
            var message = new ValidatedMessage<object>(null, null);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsNoErrors_IfValuesAreEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsEqualToOther_ReturnsExpectedError_IfValuesAreNotEqual_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThan ======]

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member + 1, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be smaller than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member - 1, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThan(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThan (Indirect) ======]

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be smaller than 'Other ({0})'.", member));
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsExpectedError_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member + 1, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsDefaultError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be smaller than or equal to '{1}'.", member, member - 1));
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsSmallerThanOrEqualTo(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsSmallerThanOrEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsDefaultError_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be smaller than or equal to 'Other ({1})'.", member, member - 1));
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsSmallerThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsSmallerThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsSmallerThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        #endregion

        #region [====== IsGreaterThan ======]

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member + 1, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than '{0}'.", member));
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member - 1, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThan(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThan (Indirect) ======]

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsDefaultError_IfMemberIsEqualToOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than 'Other ({0})'.", member));
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, null, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsExpectedError_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThan_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThan(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo ======]

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified()
        {            
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than or equal to '{1}'.", member, member + 1));
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member - 1, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(new object(), comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member + 1, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<long>(RandomErrorMessage)
                .IsGreaterThanOrEqualTo(member - 1, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsGreaterThanOrEqualTo (Indirect) ======]

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_Indirect()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedErrors_IfMemberIsSmallerThanOtherValue_And_ComparerIsNull_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, null);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be greater than or equal to 'Other ({1})'.", member, member + 1));
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsEqualToOtherValue_And_ComparerIsNull_Indirect()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_And_ComparerIsNull_Indirect()
        {
            IComparer<long> comparer = null;
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsNegativeNumber_Indirect()
        {
            // A negative result means that the specified value is smaller than the member.
            var comparer = new ComparerStub<object>(-1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsNoErrors_IfComparerReturnsZero_Indirect()
        {
            // Zero means that the specified value is equal to the member.
            var comparer = new ComparerStub<object>(0);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualToByComparer_ReturnsExpectedError_IfComparerReturnsPositiveNumber_Indirect()
        {
            // A positive result means that the specified value is greater than the member.
            var comparer = new ComparerStub<object>(1);
            var message = new ValidatedMessage<object>(new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, comparer, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsExpectedError_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member + 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsEqualToOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsGreaterThanOrEqualTo_ReturnsNoErrors_IfMemberIsGreaterThanOtherValue_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Ticks;
            var message = new ValidatedMessage<long>(member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsGreaterThanOrEqualTo(m => m.Other, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotInRange ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(null as IRange<int>, RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsNotInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsNotInRange(new object(), new object(), RandomErrorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(member, member - 1, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsExpectedError_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsDefaultError_IfMemberIsInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(0, 1000, RangeOptions.RightExclusive);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must not be within the following range: [0, 1000>.", member));
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsNoErrors_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsNotInRange(1000, 2000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsNotInRange (Indirect) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsNotInRange_Throws_IfNeitherValueImplementsComparable_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member, new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RandomErrorMessage);

            validator.Validate(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsNotInRange_Throws_IfRangeIsInvalid_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RandomErrorMessage);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsExpectedError_IfMemberIsInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 0, 1000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsDefaultError_IfMemberIsInSpecifiedRange_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 0, 1000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must not be within the following range: [0, 1000>.", member));
        }

        [TestMethod]
        public void ValidateIsNotInRange_ReturnsNoErrors_IfMemberIsNotInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 1000, 2000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsNotInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        #endregion

        #region [====== IsInRange ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateIsInRange_Throws_IfRangeIsNull()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(null as IRange<int>, RandomErrorMessage);          
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRange_Throws_IfNeitherValueImplementsComparable()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInRange(new object(), new object(), RandomErrorMessage);            
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsInRange_Throws_IfRangeIsInvalid()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(member, member - 1, RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsNoErrors_IfMemberIsInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(0, 1000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsExpectedError_IfMemberIsNotInSpecifiedRange()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsDefaultError_IfMemberIsNotInSpecifiedRange_And_NoErrorMessageIsSpecified()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member)
                .IsInstanceOf<int>(RandomErrorMessage)
                .IsInRange(1000, 2000, RangeOptions.RightExclusive);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be within the following range: [1000, 2000>.", member));
        }

        #endregion

        #region [====== IsInRange (Indirect) ======]        

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidateIsInRange_Throws_IfNeitherValueImplementsComparable_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<object>(member, new object(), new object());
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right);

            validator.Validate(message);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateIsInRange_Throws_IfRangeIsInvalid_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, member, member - 1);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right);

            validator.Validate(message);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsNoErrors_IfMemberIsInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 0, 1000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertNoErrors();
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsExpectedError_IfMemberIsNotInSpecifiedRange_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 1000, 2000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive, RandomErrorMessage);

            validator.Validate(message).AssertOneError(RandomErrorMessage);
        }

        [TestMethod]
        public void ValidateIsInRange_ReturnsDefaultError_IfMemberIsNotInSpecifiedRange_And_NoErrorMessageIsSpecified_Indirect()
        {
            var member = Clock.Current.UtcDateAndTime().Millisecond;
            var message = new ValidatedMessage<int>(member, 1000, 2000);
            var validator = message.CreateConstraintValidator();

            validator.VerifyThat(m => m.Member).IsInRange(m => m.Left, m => m.Right, RangeOptions.RightExclusive);

            validator.Validate(message).AssertOneError(string.Format("Member ({0}) must be within the following range: [1000, 2000>.", member));
        }

        #endregion
    }
}
