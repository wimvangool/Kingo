using System;

namespace Kingo.SharpDX.Direct3D
{
    internal sealed class RotatableObject : IRotatableObject, IFormattable
    {
        private readonly object _parent;
        private RotationTransformation3D _rotation;        

        public RotatableObject(object parent)
        {
            _parent = parent;
            _rotation = RotationTransformation3D.NoRotation;
        }        

        #region [====== Conversion ======]

        /// <inheritdoc />
        public override string ToString()
        {
            return _rotation.ToString();
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _rotation.ToString(format, formatProvider);
        }

        #endregion

        #region [====== Rotation =====]

        public RotationTransformation3D Rotation
        {
            get { return _rotation; }
            private set
            {
                var oldRotation = _rotation;
                var newRotation = value;

                if (newRotation != oldRotation)
                {
                    _rotation = newRotation;

                    OnRotationChanged(new PropertyChangedEventArgs<RotationTransformation3D>(oldRotation, newRotation));
                }
            }
        }        

        public event EventHandler<PropertyChangedEventArgs<RotationTransformation3D>> RotationChanged;

        private void OnRotationChanged(PropertyChangedEventArgs<RotationTransformation3D> e)
        {            
            RotationChanged.Raise(_parent, e);
        }        

        #endregion

        #region [====== Rotate ======]        

        public void Rotate(Angle x, Angle y, Angle z)
        {           
            Rotation = RotationTransformation3D.FromAngles(x, y, z) * Rotation;
        }           

        public void RotateTo(Angle x, Angle y, Angle z)
        {
            Rotation = RotationTransformation3D.FromAngles(x, y, z);
        }       

        #endregion

        #region [====== Pitch, Yaw & Roll ======]

        public void Pitch(Angle angle)
        {            
            Rotation = RotationTransformation3D.FromAngleAroundAxis(Rotation.Right, angle) * Rotation;
        }

        public void Yaw(Angle angle)
        {            
            Rotation = RotationTransformation3D.FromAngleAroundAxis(Rotation.Up, angle) * Rotation;
        }

        public void Roll(Angle angle)
        {            
            Rotation = RotationTransformation3D.FromAngleAroundAxis(Rotation.Forward, angle) * Rotation;
        }

        public void PitchYawRoll(Angle pitch, Angle yaw, Angle roll)
        {            
            Rotation =
                RotationTransformation3D.FromAngleAroundAxis(Rotation.Right, pitch) *
                RotationTransformation3D.FromAngleAroundAxis(Rotation.Up, yaw) *
                RotationTransformation3D.FromAngleAroundAxis(Rotation.Forward, roll) *
                Rotation;
        }

        #endregion
    }
}
