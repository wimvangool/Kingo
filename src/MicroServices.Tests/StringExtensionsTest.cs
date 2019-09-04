using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class StringExtensionsTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemovePostfix_Throws_IfValueIsNull()
        {
            var value = null as string;
            var postfix = Guid.NewGuid().ToString();

            value.RemovePostfix(postfix);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemovePostfix_Throws_IfPostfixIsNull()
        {
            var value = Guid.NewGuid().ToString();
            var postfix = null as string;

            value.RemovePostfix(postfix);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemovePostfix_Throws_IfComparisonIsNotValid()
        {
            var value = Guid.NewGuid().ToString();
            var postfix = value;

            value.RemovePostfix(postfix, (StringComparison) (-1));
        }

        [TestMethod]
        public void RemovePostfix_ReturnsOriginalValue_IfValueDoesNotEndWithPostfix()
        {
            var value = Guid.NewGuid().ToString();
            var postfix = Guid.NewGuid().ToString();

            Assert.AreEqual(value, value.RemovePostfix(postfix));
        }

        [TestMethod]
        public void RemovePostfix_ReturnsEmptyString_IfValueIsEqualToPostfix()
        {
            var value = Guid.NewGuid().ToString();
            var postfix = value;

            Assert.AreEqual(0, value.RemovePostfix(postfix).Length);
        }

        [TestMethod]
        public void RemovePostfix_ReturnsNewValue_IfValueEndsWithPostfix()
        {
            var value = Guid.NewGuid().ToString();
            var postfix = value.Substring(18);

            Assert.AreEqual(value.Substring(0, 18), value.RemovePostfix(postfix));
        }
    }
}
