using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX
{
    [TestClass]
    public sealed class LengthTest
    {
        #region [====== ComparableTestSuite ======]

        private static readonly ComparableTestSuite<Length> _ComparableTestSuite = new ComparableValueTypeTestSuite<Length>(new MSTestEngine());
            
        [TestMethod]
        public void ComparableTestSuite_ExecutesSuccesfully()
        {
            _ComparableTestSuite.Execute(new ComparableTestParameters<Length>()
            {
                Instance = new Length(1),
                EqualInstance = new Length(1),
                LargerInstance = new Length(2)
            });
        }

        #endregion

        #region [====== Constructor ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_Throws_IfValueIsLessThanZero()
        {
            new Length(-1);
        }

        [TestMethod]
        public void Constructor_InitializesLength_IfLengthIsZero()
        {
            var length = new Length(0);

            Assert.AreEqual(new Length(), length);
        }

        [TestMethod]
        public void Constructor_InitializesLength_IfLenghtIsGreaterThanZero()
        {
            var length = new Length(1);

            Assert.AreEqual(1f, length.ToSingle());
        }

        #endregion               
    }
}
