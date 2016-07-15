using System;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented by a class, represents an object that can be rotated in three dimensions.
    /// </summary>
    public interface IRotatableObject
    {
        #region [====== Rotation ======]

        /// <summary>
        /// The current rotation-transformation of the object.
        /// </summary>
        RotationTransformation3D Rotation
        {
            get;
        }

        /// <summary>
        /// Event that is raised when <see cref="Rotation"/> has changed.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs<RotationTransformation3D>> RotationChanged;

        #endregion

        #region [====== Rotate ======]       

        /// <summary>
        /// Rotates the object using the X-, Y and Z-axis as the rotation-axes.
        /// </summary>
        /// <param name="x">The angle of rotation along the X-axis.</param>
        /// <param name="y">The angle of rotation along the Y-axis.</param>
        /// <param name="z">The angle of rotation along the Z-axis.</param>
        void Rotate(Angle x, Angle y, Angle z);     

        /// <summary>
        /// Rotates the object such that is has exactly the specified rotation-angles with respect to the X, Y- and Z-axes.
        /// </summary>
        /// <param name="x">The angle of rotation along the X-axis.</param>
        /// <param name="y">The angle of rotation along the Y-axis.</param>
        /// <param name="z">The angle of rotation along the Z-axis.</param>
        void RotateTo(Angle x, Angle y, Angle z);        

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

        /// <summary>
        /// Rotates the object using its local X-, Y- and Z-axes.
        /// </summary>
        /// <param name="pitch">The pitch-angle.</param>
        /// <param name="yaw">The yaw-angle.</param>
        /// <param name="roll">The roll-angle.</param>
        void PitchYawRoll(Angle pitch, Angle yaw, Angle roll);

        #endregion     
    }
}
