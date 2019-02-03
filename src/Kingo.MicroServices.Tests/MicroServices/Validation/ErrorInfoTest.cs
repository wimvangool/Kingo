using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Validation
{
    [TestClass]
    public sealed class ErrorInfoTest
    {
        #region [====== Merge ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Merge_Throws_IfLeftIsNull()
        {
            ErrorInfo.Merge(null, ErrorInfo.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Merge_Throws_IfRightIsNull()
        {
            ErrorInfo.Merge(ErrorInfo.Empty, null);
        }

        [TestMethod]
        public void Merge_ReturnsEmptyInstance_IfBothLeftAndRightAreEmpty()
        {
            var left = new ErrorInfo(new Dictionary<string, string>());
            var right = new ErrorInfo(new Dictionary<string, string>());
            var errorInfo = ErrorInfo.Merge(left, right);

            Assert.AreSame(ErrorInfo.Empty, errorInfo);
        }

        [TestMethod]
        public void Merge_ReturnsCorrectlyMergedResult_IfBothLeftAndRightContainMemberErrors_And_ErrorsDoNotOverlap()
        {
            var leftErrors = new Dictionary<string, string>
            {
                { "A", "left" }
            };
            var left = new ErrorInfo(leftErrors);

            var rightErrors = new Dictionary<string, string>
            {
                { "B", "right" }
            };
            var right = new ErrorInfo(rightErrors);

            var errorInfo = ErrorInfo.Merge(left, right);

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(2, errorInfo.MemberErrors.Count);
            Assert.AreEqual("left", errorInfo.MemberErrors["A"]);
            Assert.AreEqual("right", errorInfo.MemberErrors["B"]);
        }

        [TestMethod]
        public void Merge_ReturnsCorrectlyMergedResult_IfBothLeftAndRightContainMemberErrors_And_ErrorsOverlap()
        {
            var leftErrors = new Dictionary<string, string>
            {
                { "A", "left" }
            };
            var left = new ErrorInfo(leftErrors);

            var rightErrors = new Dictionary<string, string>
            {
                { "A", "right" }, { "B", "right" }
            };
            var right = new ErrorInfo(rightErrors);

            var errorInfo = ErrorInfo.Merge(left, right);

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(2, errorInfo.MemberErrors.Count);
            Assert.AreEqual("left", errorInfo.MemberErrors["A"]);
            Assert.AreEqual("right", errorInfo.MemberErrors["B"]);
        }

        [TestMethod]
        public void Merge_ReturnsCorrectlyMergedResult_IfLeftContainsInstanceError_And_RightDoesNot()
        {
            var left = new ErrorInfo(new Dictionary<string, string>(), "left");
            var right = new ErrorInfo(new Dictionary<string, string>());

            var errorInfo = ErrorInfo.Merge(left, right);

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(0, errorInfo.MemberErrors.Count);
            Assert.AreEqual("left", errorInfo.Error);
        }

        [TestMethod]
        public void Merge_ReturnsCorrectlyMergedResult_IfRightContainsInstanceError_And_LeftDoesNot()
        {
            var left = new ErrorInfo(new Dictionary<string, string>());
            var right = new ErrorInfo(new Dictionary<string, string>(), "right");

            var errorInfo = ErrorInfo.Merge(left, right);

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(0, errorInfo.MemberErrors.Count);
            Assert.AreEqual("right", errorInfo.Error);
        }

        [TestMethod]
        public void Merge_ReturnsCorrectlyMergedResult_IfBothLeftAndRightContainInstanceError()
        {
            var left = new ErrorInfo(new Dictionary<string, string>(), "left");
            var right = new ErrorInfo(new Dictionary<string, string>(), "right");

            var errorInfo = ErrorInfo.Merge(left, right);

            Assert.IsNotNull(errorInfo);
            Assert.AreEqual(0, errorInfo.MemberErrors.Count);
            Assert.AreEqual("left", errorInfo.Error);
        }

        #endregion
    }
}
