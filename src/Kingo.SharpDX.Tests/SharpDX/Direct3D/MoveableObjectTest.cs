using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.SharpDX.Direct3D
{
    [TestClass]
    public sealed class MoveableObjectTest
    {
        private MoveableObject _moveableObject;
        private bool _eventWasRaised;
        private Position3D _oldPosition;
        private Position3D _newPosition;

        [TestInitialize]
        public void Setup()
        {
            _moveableObject = new MoveableObject(this);
            _moveableObject.PositionChanged += (s, e) =>
            {
                _eventWasRaised = true;
                _oldPosition = e.OldPosition;
                _newPosition = e.NewPosition;
            };
        }

        #region [====== Move ======]

        [TestMethod]
        public void MoveX_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var x = RandomDistance();

            _moveableObject.MoveX(x);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Position3D.Origin, _oldPosition);
            Assert.AreEqual(new Position3D(x, 0, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveX_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var x0 = RandomDistance();
            var x1 = RandomDistance();

            _moveableObject.MoveX(x0);
            _moveableObject.MoveX(x1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new Position3D(x0, 0, 0), _oldPosition);
            Assert.AreEqual(new Position3D(x0 + x1, 0, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveY_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var y = RandomDistance();

            _moveableObject.MoveY(y);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Position3D.Origin, _oldPosition);
            Assert.AreEqual(new Position3D(0, y, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveY_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var y0 = RandomDistance();
            var y1 = RandomDistance();

            _moveableObject.MoveY(y0);
            _moveableObject.MoveY(y1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new Position3D(0, y0, 0), _oldPosition);
            Assert.AreEqual(new Position3D(0, y0 + y1, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveZ_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var z = RandomDistance();

            _moveableObject.MoveZ(z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Position3D.Origin, _oldPosition);
            Assert.AreEqual(new Position3D(0, 0, z), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveZ_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var z0 = RandomDistance();
            var z1 = RandomDistance();

            _moveableObject.MoveZ(z0);
            _moveableObject.MoveZ(z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new Position3D(0, 0, z0), _oldPosition);
            Assert.AreEqual(new Position3D(0, 0, z0 + z1), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        #endregion

        #region [====== MoveTo ======]

        [TestMethod]
        public void MoveToX_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var x = RandomDistance();

            _moveableObject.MoveToX(x);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Position3D.Origin, _oldPosition);
            Assert.AreEqual(new Position3D(x, 0, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveToX_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var x0 = RandomDistance();
            var x1 = RandomDistance();

            _moveableObject.MoveToX(x0);
            _moveableObject.MoveToX(x1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new Position3D(x0, 0, 0), _oldPosition);
            Assert.AreEqual(new Position3D(x1, 0, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveToY_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var y = RandomDistance();

            _moveableObject.MoveToY(y);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Position3D.Origin, _oldPosition);
            Assert.AreEqual(new Position3D(0, y, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveToY_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var y0 = RandomDistance();
            var y1 = RandomDistance();

            _moveableObject.MoveToY(y0);
            _moveableObject.MoveToY(y1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new Position3D(0, y0, 0), _oldPosition);
            Assert.AreEqual(new Position3D(0, y1, 0), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveToZ_MovesObjectToExpectedPosition_IfCurrentIsOrigin()
        {
            var z = RandomDistance();

            _moveableObject.MoveToZ(z);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(Position3D.Origin, _oldPosition);
            Assert.AreEqual(new Position3D(0, 0, z), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
        }

        [TestMethod]
        public void MoveToZ_MovesObjectToExpectedPosition_IfCurrentIsNotOrigin()
        {
            var z0 = RandomDistance();
            var z1 = RandomDistance();

            _moveableObject.MoveToZ(z0);
            _moveableObject.MoveToZ(z1);

            Assert.IsTrue(_eventWasRaised);
            Assert.AreEqual(new Position3D(0, 0, z0), _oldPosition);
            Assert.AreEqual(new Position3D(0, 0, z1), _newPosition);
            Assert.AreEqual(_moveableObject.Position, _newPosition);
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
