using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class MoveableObjectTest
    {
        private MoveableObject _moveableObject;
        private bool _eventWasRaised;
        private TranslationTransformation3D _oldPosition;
        private TranslationTransformation3D _newPosition;

        [TestInitialize]
        public void Setup()
        {
            _moveableObject = new MoveableObject(this);
            _moveableObject.TranslationChanged += (s, e) =>
            {
                _eventWasRaised = true;
                _oldPosition = e.OldValue;
                _newPosition = e.NewValue;
            };
        }

        #region [====== Move ======]

        [TestMethod]
        public void Move_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var x = RandomDistance();
            var y = RandomDistance();
            var z = RandomDistance();

            _moveableObject.Move(x, y, z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(TranslationTransformation3D.NoTranslation, _oldPosition);
            Assert.AreEqual(new TranslationTransformation3D(x, y, z), _newPosition);
            Assert.AreEqual(_moveableObject.Translation, _newPosition);
        }

        [TestMethod]
        public void Move_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var x0 = RandomDistance();
            var y0 = RandomDistance();
            var z0 = RandomDistance();

            var x1 = RandomDistance();
            var y1 = RandomDistance();
            var z1 = RandomDistance();

            _moveableObject.Move(x0, y0, z0);
            _moveableObject.Move(x1, y1, z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new TranslationTransformation3D(x0, y0, z0), _oldPosition);
            Assert.AreEqual(new TranslationTransformation3D(x0 + x1, y0 + y1, z0 + z1), _newPosition);
            Assert.AreEqual(_moveableObject.Translation, _newPosition);
        }
                
        #endregion

        #region [====== MoveTo ======]

        [TestMethod]
        public void MoveTo_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var x = RandomDistance();
            var y = RandomDistance();
            var z = RandomDistance();

            _moveableObject.MoveTo(x, y, z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(TranslationTransformation3D.NoTranslation, _oldPosition);
            Assert.AreEqual(new TranslationTransformation3D(x, y, z), _newPosition);
            Assert.AreEqual(_moveableObject.Translation, _newPosition);
        }

        [TestMethod]
        public void MoveTo_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var x0 = RandomDistance();
            var y0 = RandomDistance();
            var z0 = RandomDistance();

            var x1 = RandomDistance();
            var y1 = RandomDistance();
            var z1 = RandomDistance();

            _moveableObject.MoveTo(x0, y0, z0);
            _moveableObject.MoveTo(x1, y1, z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new TranslationTransformation3D(x0, y0, z0), _oldPosition);
            Assert.AreEqual(new TranslationTransformation3D(x1, y1, z1), _newPosition);
            Assert.AreEqual(_moveableObject.Translation, _newPosition);
        }      

        #endregion

        private static readonly Random _RandomValueGenerator = new Random();

        private static float RandomDistance()
        {
            lock (_RandomValueGenerator)
            {
                return _RandomValueGenerator.Next(-1000, 1000);
            }
        }
    }
}
