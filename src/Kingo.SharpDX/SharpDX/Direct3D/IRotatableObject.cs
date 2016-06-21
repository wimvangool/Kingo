using System;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented by a class, represents an object in 3D-space that can be rotated with three degrees of freedom.
    /// </summary>
    public interface IRotatableObject
    {
        #region [====== Rotation ======]

        /// <summary>
        /// The current rotation of the object.
        /// </summary>
        Rotation3D Rotation
        {
            get;
        }

        /// <summary>
        /// Event that is raised when <see cref="Rotation"/> has changed.
        /// </summary>
        event EventHandler<Rotation3DChangedEventArgs> RotationChanged;

        #endregion

        #region [====== Rotate (Relative) ======]

        /// <summary>
        /// Rotates the object using the X-axis as the rotation-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void RotateX(Angle angle);

        /// <summary>
        /// Rotates the object using the Y-axis as the rotation-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void RotateY(Angle angle);

        /// <summary>
        /// Rotates the object using the Z-axis as the rotation-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void RotateZ(Angle angle);

        /// <summary>
        /// Rotates the object using the X-, Y and Z-axis as the rotation-axes.
        /// </summary>
        /// <param name="x">The angle of rotation along the X-axis.</param>
        /// <param name="y">The angle of rotation along the Y-axis.</param>
        /// <param name="z">The angle of rotation along the Z-axis.</param>
        void Rotate(Angle x, Angle y, Angle z);

        #endregion

        #region [====== RotateTo (Absolute) ======]

        /// <summary>
        /// Rotates the object such that is has exactly the specified rotation-angle with respect to the X-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void RotateToX(Angle angle);

        /// <summary>
        /// Rotates the object such that is has exactly the specified rotation-angle with respect to the Y-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void RotateToY(Angle angle);

        /// <summary>
        /// Rotates the object such that is has exactly the specified rotation-angle with respect to the Z-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void RotateToZ(Angle angle);

        /// <summary>
        /// Rotates the object such that is has exactly the specified rotation-angles with respect to the X, Y- and Z-axes.
        /// </summary>
        /// <param name="x">The angle of rotation along the X-axis.</param>
        /// <param name="y">The angle of rotation along the Y-axis.</param>
        /// <param name="z">The angle of rotation along the Z-axis.</param>
        void RotateTo(Angle x, Angle y, Angle z);

        /// <summary>
        /// Sets the rotation of the object exactly to the specified <paramref name="rotation"/>.
        /// </summary>
        /// <param name="rotation">The desired rotation of the object.</param>
        void RotateTo(Rotation3D rotation);

        #endregion

        #region [====== Pitch / Yaw / Roll ======]

        /// <summary>
        /// Rotates the object using its local X-axis (the right-vector with respect to its current rotation) as the rotation-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void Pitch(Angle angle);

        /// <summary>
        /// Rotates the object using its local Y-axis (the up-vector with respect to its current rotation) as the rotation-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void Yaw(Angle angle);

        /// <summary>
        /// Rotates the object using its local Z-axis (the forward-vector with respect to its current rotation) as the rotation-axis.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        void Roll(Angle angle);

        #endregion     
    }
}
