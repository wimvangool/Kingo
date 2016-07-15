using System;
using SharpDX;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented by a class, represents an object that can be moved in three dimensions.
    /// </summary>
    public interface IMoveableObject
    {
        #region [====== Translation ======]

        /// <summary>
        /// The current translation-transformation of the object.
        /// </summary>
        TranslationTransformation3D Translation
        {
            get;
        }

        /// <summary>
        /// Event that is raised when <see cref="Translation"/> has changed.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs<TranslationTransformation3D>> TranslationChanged;

        #endregion

        #region [====== Move ======]        

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

        #endregion
    }
}
