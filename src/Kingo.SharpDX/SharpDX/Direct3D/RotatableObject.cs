using System;

namespace Kingo.SharpDX.Direct3D
{
    internal sealed class RotatableObject : IRotatableObject, IFormattable
    {
        private readonly object _parent;
        private Rotation3D _rotation;        

        public RotatableObject(object parent)
        {
            _parent = parent;
            _rotation = Rotation3D.NoRotation;
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

        public Rotation3D Rotation
        {
            get { return _rotation; }
            private set
            {
                var oldRotation = _rotation;
                var newRotation = value;

                if (newRotation != oldRotation)
                {
                    _rotation = newRotation;

                    OnRotationChanged(new Rotation3DChangedEventArgs(oldRotation, newRotation));
                }
            }
        }        

        public event EventHandler<Rotation3DChangedEventArgs> RotationChanged;

        private void OnRotationChanged(Rotation3DChangedEventArgs e)
        {            
            RotationChanged.Raise(_parent, e);
        }        

        #endregion

        #region [====== Rotate (Relative) ======]

        public void RotateX(Angle angle)
        {
            Rotate(angle, Angle.Zero, Angle.Zero);
        }

        public void RotateY(Angle angle)
        {
            Rotate(Angle.Zero, angle, Angle.Zero);
        }

        public void RotateZ(Angle angle)
        {
            Rotate(Angle.Zero, Angle.Zero, angle);
        }

        public void Rotate(Angle x, Angle y, Angle z)
        {           
            Rotation = Rotation3D.FromAngles(x, y, z) * Rotation;
        }

        #endregion

        #region [====== RotateTo (Absolute) =======]

        public void RotateToX(Angle angle)
        {
            RotateTo(angle, Rotation.AroundY, Rotation.AroundZ);
        }

        public void RotateToY(Angle angle)
        {
            RotateTo(Rotation.AroundX, angle, Rotation.AroundZ);
        }

        public void RotateToZ(Angle angle)
        {
            RotateTo(Rotation.AroundX, Rotation.AroundY, angle);
        }

        public void RotateTo(Angle x, Angle y, Angle z)
        {
            RotateTo(Rotation3D.FromAngles(x, y, z));
        }

        public void RotateTo(Rotation3D rotation)
        {
            Rotation = rotation;
        }

        #endregion

        #region [====== Pitch, Yaw & Roll ======]

        public void Pitch(Angle angle)
        {            
            Rotation = Rotation3D.FromAngleAroundAxis(Rotation.Right, angle) * Rotation;
        }

        public void Yaw(Angle angle)
        {            
            Rotation = Rotation3D.FromAngleAroundAxis(Rotation.Up, angle) * Rotation;
        }

        public void Roll(Angle angle)
        {            
            Rotation = Rotation3D.FromAngleAroundAxis(Rotation.Forward, angle) * Rotation;
        }                       

        #endregion
    }
}
