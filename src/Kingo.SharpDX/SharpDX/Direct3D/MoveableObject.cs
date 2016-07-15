using System;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    internal sealed class MoveableObject : IMoveableObject
    {
        private readonly object _parent;
        private TranslationTransformation3D _translation;

        public MoveableObject(object parent)
        {
            _parent = parent;
        }

        #region [====== Translation ======]

        public TranslationTransformation3D Translation
        {
            get { return _translation; }
            private set
            {
                var oldPosition = _translation;
                var newPosition = value;

                if (newPosition != oldPosition)
                {
                    _translation = newPosition;

                    OnPositionChanged(new PropertyChangedEventArgs<TranslationTransformation3D>(oldPosition, newPosition));
                }
            }
        }

        public event EventHandler<PropertyChangedEventArgs<TranslationTransformation3D>> TranslationChanged;

        private void OnPositionChanged(PropertyChangedEventArgs<TranslationTransformation3D> e)
        {
            TranslationChanged.Raise(_parent, e);
        }

        #endregion

        #region [====== Move (Relative) ======]       

        public void Move(Vector3 direction)
        {
            Move(direction.X, direction.Y, direction.Z);
        }

        public void Move(float x, float y, float z)
        {
            Translation = new TranslationTransformation3D(Translation.X + x, Translation.Y + y, Translation.Z + z);
        }

        #endregion

        #region [====== MoveTo (Absolute) ======]      

        public void MoveTo(Vector3 position)
        {
            MoveTo(new TranslationTransformation3D(position));
        }

        public void MoveTo(float x, float y, float z)
        {
            MoveTo(new TranslationTransformation3D(x, y, z));
        }        

        public void MoveTo(TranslationTransformation3D position)
        {
            Translation = position;
        }

        #endregion
    }
}
