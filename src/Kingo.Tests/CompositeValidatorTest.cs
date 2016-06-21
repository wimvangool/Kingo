using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class CompositeValidatorTest
    {
        #region [====== Merge ======]

        [TestMethod]        
        public void Merge_ReturnsRight_IfLeftIsNull()
        {
            IValidator left = null;
            IValidator right = new NullValidator();

            Assert.AreSame(right, CompositeValidator.Merge(left, right));
        }

        [TestMethod]        
        public void Merge_ReturnsLeft_IfRightIsNull()
        {
            IValidator left = new NullValidator();
            IValidator right = null;

            Assert.AreSame(left, CompositeValidator.Merge(left, right));
        }

        [TestMethod]
        public void Merge_ReturnsNullValidator_IfBothLeftAndRightAreNull()
        {
            var validator = CompositeValidator.Merge(null, null);

            Assert.IsNotNull(validator);
            Assert.AreEqual(typeof(NullValidator), validator.GetType());
        }

        [TestMethod]        
        public void MergeWith_ReturnsCompositeValidator_IfBothLeftAndRightAreNotNull()
        {
            Assert.IsNotNull(CompositeValidator.Merge(new NullValidator(), new NullValidator()));
        }

        #endregion

        #region [====== Validate ======]

        private sealed class ValidatorStub : IValidator
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
                    return ErrorInfo.Empty;
                }
                return _errorInfo;
            }

            public IValidator MergeWith(IValidator validator, bool haltOnFirstError = false)
            {
                return CompositeValidator.Merge(this, validator, haltOnFirstError);
            }
        }

        [TestMethod]
        public void Validate_ReturnsErrorOfLeftOnly_IfHaltOnFirstErrorIsTrue()
        {
            var left = new ValidatorStub("left");
            var right = new ValidatorStub("right");
            var composite = left.MergeWith(right, true);

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
            var composite = left.MergeWith(right, true);

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
            var composite = left.MergeWith(right);

            var errorInfo = composite.Validate(new object());

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(2, errorInfo.ErrorCount);
            Assert.AreEqual("left", errorInfo.MemberErrors["A"]);
            Assert.AreEqual("right", errorInfo.MemberErrors["B"]);
        }

        #endregion
    }
}
