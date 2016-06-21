using System;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Contains both the new and previous position of an object.
    /// </summary>
    public sealed class Position3DChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Position3DChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldPosition">The old position of the object.</param>
        /// <param name="newPosition">The new position of the object.</param>
        public Position3DChangedEventArgs(Position3D oldPosition, Position3D newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        /// <summary>
        /// The old position of the object.
        /// </summary>
        public Position3D OldPosition
        {
            get;
        }

        /// <summary>
        /// The new position of the object.
        /// </summary>
        public Position3D NewPosition
        {
            get;
        }
    }
}
