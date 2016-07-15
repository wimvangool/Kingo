﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class TranslationTransformation3DTest
    {
        #region [====== EquatableTestSuite ======]

        private static readonly EquatableValueTypeTestSuite<TranslationTransformation3D> _EquatableTestSuite = new EquatableValueTypeTestSuite<TranslationTransformation3D>(new MSTestEngine());

        [TestMethod]
        public void EquatableTestSuite_ExecutesSuccesfully()
        {
            _EquatableTestSuite.Execute(new EquatableTestParameters<TranslationTransformation3D>()
            {
                Instance = new TranslationTransformation3D(1, 2, 3),
                EqualInstance = new TranslationTransformation3D(1, 2, 3),
                UnequalInstance = new TranslationTransformation3D(2, 3, 4)
            });
        }

        #endregion

        #region [====== Constructors ======]

        [TestMethod]
        public void Constructor_ByFloats_InitializesPositionCorrectly()
        {
            var position = new TranslationTransformation3D(3, 4, 5);

            Assert.AreEqual(3, position.X);
            Assert.AreEqual(4, position.Y);
            Assert.AreEqual(5, position.Z);
        }

        [TestMethod]
        public void Constructor_ByVector_InitializesPositionCorrectly()
        {
            var position = new TranslationTransformation3D(new Vector3(3, 4, 5));

            Assert.AreEqual(3, position.X);
            Assert.AreEqual(4, position.Y);
            Assert.AreEqual(5, position.Z);
        }

        #endregion

        #region [====== ToString ======]

        [TestMethod]
        public void ToString_ReturnsExpectedStringRepresentation()
        {
            var position = new TranslationTransformation3D(6, 7, 8); 
            
            Assert.AreEqual("[6 | 7 | 8]", position.ToString());  
        }

        #endregion

        #region [====== TranslationMatrix ======]

        [TestMethod]
        public void TranslationMatrix_ReturnsIdentityMatrix_IfPositionIsOrigin()
        {
            var position = TranslationTransformation3D.NoTranslation;
            var matrix = position.TransformationMatrix;
            
            Assert.AreEqual(Matrix.Identity, matrix);
        }

        [TestMethod]
        public void TranslationMatrix_ReturnsExpectedMatrix_IfPositionIsNotTheOrigin()
        {
            var position = new TranslationTransformation3D(6, 7, 8);
            var matrix = position.TransformationMatrix;

            Assert.AreEqual(1, matrix.M11);
            Assert.AreEqual(0, matrix.M12);
            Assert.AreEqual(0, matrix.M13);
            Assert.AreEqual(0, matrix.M14);

            Assert.AreEqual(0, matrix.M21);
            Assert.AreEqual(1, matrix.M22);
            Assert.AreEqual(0, matrix.M23);
            Assert.AreEqual(0, matrix.M24);

            Assert.AreEqual(0, matrix.M31);
            Assert.AreEqual(0, matrix.M32);
            Assert.AreEqual(1, matrix.M33);
            Assert.AreEqual(0, matrix.M34);                       

            Assert.AreEqual(6, matrix.M41);
            Assert.AreEqual(7, matrix.M42);
            Assert.AreEqual(8, matrix.M43);
            Assert.AreEqual(1, matrix.M44);
        }

        #endregion
    }
}