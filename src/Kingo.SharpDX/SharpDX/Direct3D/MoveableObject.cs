using System;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    internal sealed class MoveableObject : IMoveableObject
    {
        private readonly object _parent;
        private Position3D _position;

        public MoveableObject(object parent)
        {
            _parent = parent;
        }

        #region [====== Position ======]

        public Position3D Position
        {
            get { return _position; }
            private set
            {
                var oldPosition = _position;
                var newPosition = value;

                if (newPosition != oldPosition)
                {
                    _position = newPosition;

                    OnPositionChanged(new Position3DChangedEventArgs(oldPosition, newPosition));
                }
            }
        }

        public event EventHandler<Position3DChangedEventArgs> PositionChanged;

        private void OnPositionChanged(Position3DChangedEventArgs e)
        {
            PositionChanged.Raise(_parent, e);
        }

        #endregion

        #region [====== Move (Relative) ======]

        public void MoveX(float x)
        {
            Move(x, 0, 0);
        }

        public void MoveY(float y)
        {
            Move(0, y, 0);
        }

        public void MoveZ(float z)
        {
            Move(0, 0, z);
        }        

        public void Move(Vector3 direction)
        {
            Move(direction.X, direction.Y, direction.Z);
        }

        public void Move(float x, float y, float z)
        {
            Position = new Position3D(Position.X + x, Position.Y + y, Position.Z + z);
        }

        #endregion

        #region [====== MoveTo (Absolute) ======]

        public void MoveToX(float x)
        {
            MoveTo(x, Position.Y, Position.Z);
        }

        public void MoveToY(float y)
        {
            MoveTo(Position.X, y, Position.Z);
        }

        public void MoveToZ(float z)
        {
            MoveTo(Position.X, Position.Y, z);
        }

        public void MoveTo(Vector3 position)
        {
            MoveTo(new Position3D(position));
        }

        public void MoveTo(float x, float y, float z)
        {
            MoveTo(new Position3D(x, y, z));
        }        

        public void MoveTo(Position3D position)
        {
            Position = position;
        }

        #endregion
    }
}
