using System;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// Contains both the new and previous rotation of an object.
    /// </summary>
    public sealed class Rotation3DChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rotation3DChangedEventArgs" /> class.
        /// </summary>
        /// <param name="oldRotation">The old rotation of the object.</param>
        /// <param name="newRotation">The new rotation of the object.</param>
        public Rotation3DChangedEventArgs(Rotation3D oldRotation, Rotation3D newRotation)
        {
            OldRotation = oldRotation;
            NewRotation = newRotation;
        }

        /// <summary>
        /// The old rotation of the object.
        /// </summary>
        public Rotation3D OldRotation
        {
            get;
        }

        /// <summary>
        /// The new rotation of the object.
        /// </summary>
        public Rotation3D NewRotation
        {
            get;
        }
    }
}
