using System;
using System.Collections.Generic;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class CompositeValidatorTest
    {
        #region [====== Append ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_Throws_IfLeftIsNull()
        {
            CompositeValidator<object>.Append(null, new NullValidator<object>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Append_Throws_IfRightIsNull()
        {
            CompositeValidator<object>.Append(new NullValidator<object>(), null);
        }

        [TestMethod]        
        public void Append_ReturnsCompositeValidator_IfBothLeftAndRightAreNotNull()
        {
            Assert.IsNotNull(CompositeValidator<object>.Append(new NullValidator<object>(), new NullValidator<object>()));
        }

        #endregion

        #region [====== Validate ======]

        private sealed class ValidatorStub : IValidator<object>
        {
            private readonly ErrorInfo _errorInfo;

            public ValidatorStub(string error)
            {
                _errorInfo = new ErrorInfo(new Dictionary<string, string>(), error);
            }

            public ValidatorStub(string error, string memberName)
            {
                _errorInfo = new ErrorInfo(new Dictionary<string, string>
                { { memberName, error} });
            }

            public ErrorInfo Validate(object instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException("instance");
                }
                return _errorInfo;
            }

            public IValidator<object> Append(IValidator<object> validator, bool haltOnFirstError = false)
            {
                return CompositeValidator<object>.Append(this, validator, haltOnFirstError);
            }
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfLeftOnly_IfHaltOnFirstErrorIsTrue()
        {
            var left = new ValidatorStub("left");
            var right = new ValidatorStub("right");
            var composite = left.Append(right, true);

            var errorInfo = composite.Validate(new object());

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(1, errorInfo.ErrorCount);
            Assert.AreEqual("left", errorInfo.Error);
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfRightOnly_IfHaltOnFirstErrorIsTrue_And_LeftProducesNoErrors()
        {
            var left = new ValidatorStub(null);
            var right = new ValidatorStub("right");
            var composite = left.Append(right, true);

            var errorInfo = composite.Validate(new object());

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(1, errorInfo.ErrorCount);
            Assert.AreEqual("right", errorInfo.Error);
        }

        [TestMethod]
        public void Validate_ReturnsErrorsOfBothLeftAndRight_IfHaltOnFirstErrorIsFalse_And_BothValidatorsProduceErrors()
        {
            var left = new ValidatorStub("left", "A");
            var right = new ValidatorStub("right", "B");
            var composite = left.Append(right);

            var errorInfo = composite.Validate(new object());

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(2, errorInfo.ErrorCount);
            Assert.AreEqual("left", errorInfo.MemberErrors["A"]);
            Assert.AreEqual("right", errorInfo.MemberErrors["B"]);
        }

        #endregion
    }
}
