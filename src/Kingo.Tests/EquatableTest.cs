using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo
{
    [TestClass]
    public sealed class EquatableTest
    {
        [TestMethod]
        public void Equals_ReturnsTrue_IfLeftIsNull_And_RightIsNull()
        {
            Assert.IsTrue(Equatable.Equals<string>(null, null));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfLeftIsNull_And_RightIsNot()
        {
            Assert.IsFalse(Equatable.Equals(null, string.Empty));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfLeftIsNotNull_And_RightIsNull()
        {
            Assert.IsFalse(Equatable.Equals(string.Empty, null));
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfLeftIsEqualToRight()
        {
            Assert.IsTrue(Equatable.Equals(string.Empty, string.Empty));
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfLeftIsNotEqualToRight()
        {
            Assert.IsFalse(Equatable.Equals(string.Empty, " "));
        }
    }
}
