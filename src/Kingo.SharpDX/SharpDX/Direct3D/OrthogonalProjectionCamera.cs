using System;
using System.Drawing;
using Kingo.Resources;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Represents a <see cref="Camera" /> that provides an orthogonal view on the virtual world.
    /// </summary>
    public class OrthogonalProjectionCamera : Camera
    {
        private Size _screenSize = new Size(800, 600);            

        /// <summary>
        /// Gets or sets the size of the screen the projection must be plotted.
        /// </summary>
        public Size ScreenSize
        {
            get { return _screenSize; }
            set
            {
                if (value.Width == 0)
                {
                    throw NewEmptyScreenSizeException(nameof(value.Width), value);
                }
                if (value.Height == 0)
                {
                    throw NewEmptyScreenSizeException(nameof(value.Height), value);
                }
                _screenSize = value;
            }
        }

        private static Exception NewEmptyScreenSizeException(string paramName, object size)
        {
            var messageFormat = ExceptionMessages.OrthogonalCamera_InvalidScreenSize;
            var message = string.Format(messageFormat, size);
            return new ArgumentOutOfRangeException(paramName, message);
        }

        /// <inheritdoc />
        protected override Matrix ProjectionMatrix
        {
            get { return Matrix.OrthoLH(_screenSize.Width, _screenSize.Height, Planes.NearPlane, Planes.FarPlane); }
        }
    }
}
