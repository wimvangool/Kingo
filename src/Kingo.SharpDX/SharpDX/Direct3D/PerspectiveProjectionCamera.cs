using System;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a <see cref="Camera" /> that provides a perspective view on the virtual world.
    /// </summary>
    public class PerspectiveProjectionCamera : Camera
    {
        private FieldOfVision _fieldOfVision = FieldOfVision.Default;
        private AspectRatio _aspectRatio = AspectRatio.Default;              

        /// <summary>
        /// Gets or sets the field-or-vision (FoV) of the camera.
        /// </summary>
        public FieldOfVision FieldOfVision
        {
            get { return _fieldOfVision; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _fieldOfVision = value;
            }
        }

        /// <summary>
        /// Gets or sets the aspect ratio of the camera.
        /// </summary>
        public AspectRatio AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _aspectRatio = value;
            }
        }

        /// <inheritdoc />
        protected override Matrix ProjectionMatrix
        {
            get { return Matrix.PerspectiveFovLH(_fieldOfVision.ToRadians(), _aspectRatio, Planes.NearPlane, Planes.FarPlane); }
        }
    }
}
