using System;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented by a class, represents an object in 3D-space that can be moved from one position to another.
    /// </summary>
    public interface IMoveableObject
    {
        #region [====== Position ======]

        /// <summary>
        /// The current position of the object.
        /// </summary>
        Position3D Position
        {
            get;
        }

        /// <summary>
        /// Event that is raised when <see cref="Position"/> has changed.
        /// </summary>
        event EventHandler<Position3DChangedEventArgs> PositionChanged;

        #endregion

        #region [====== Move (Relative) ======]

        /// <summary>
        /// Moves the object <paramref name="x"/> places along the X-axis, relative to its current position.
        /// </summary>
        /// <param name="x">The amount of places to move.</param>
        void MoveX(float x);

        /// <summary>
        /// Moves the object <paramref name="y"/> places along the Y-axis, relative to its current position.
        /// </summary>
        /// <param name="y">The amount of places to move.</param>
        void MoveY(float y);

        /// <summary>
        /// Moves the object <paramref name="z"/> places along the Z-axis, relative to its current position.
        /// </summary>
        /// <param name="z">The amount of places to move.</param>
        void MoveZ(float z);

        /// <summary>
        /// Moves the object to another location relative to its current position.
        /// </summary>
        /// <param name="x">The amount of places to move along the X-axis.</param>
        /// <param name="y">The amount of places to move along the Y-axis.</param>
        /// <param name="z">The amount of places to move along the Z-axis.</param>
        void Move(float x, float y, float z);

        /// <summary>
        /// Moves the object to another location relative to its current position.
        /// </summary>
        /// <param name="direction">
        /// A vector representing the direction to move in.
        /// </param>
        void Move(Vector3 direction);

        #endregion

        #region [====== MoveTo (Absolute) ======]

        /// <summary>
        /// Moves the object to point <paramref name="x"/> of the X-axis.
        /// </summary>
        /// <param name="x">The X-coordinate to move to.</param>
        void MoveToX(float x);

        /// <summary>
        /// Moves the object to point <paramref name="y"/> of the Y-axis.
        /// </summary>
        /// <param name="y">The Y-coordinate to move to.</param>
        void MoveToY(float y);

        /// <summary>
        /// Moves the object to point <paramref name="z"/> of the Z-axis.
        /// </summary>
        /// <param name="z">The Z-coordinate to move to.</param>
        void MoveToZ(float z);

        /// <summary>
        /// Moves the object to the specified position.
        /// </summary>
        /// <param name="x">Location on the X-axis.</param>
        /// <param name="y">Location on the Y-axis.</param>
        /// <param name="z">Location on the Z-axis.</param>
        void MoveTo(float x, float y, float z);

        /// <summary>
        /// Moves the object to the specified position.
        /// </summary>
        /// <param name="position">The position to move to.</param>
        void MoveTo(Vector3 position);

        /// <summary>
        /// Moves the object to the specified position.
        /// </summary>
        /// <param name="position">The position to move to.</param>
        void MoveTo(Position3D position);

        #endregion
    }
}
