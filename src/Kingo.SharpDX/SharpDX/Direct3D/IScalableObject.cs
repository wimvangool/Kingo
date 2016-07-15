using System;

namespace Kingo.SharpDX.Direct3D
{
    /// <summary>
    /// When implemented by a class, represents an object that can be resized in three dimensions.
    /// </summary>
    public interface IScalableObject
    {
        #region [====== Scale ======]

        /// <summary>
        /// The current scale-transformation of the object.
        /// </summary>
        ScaleTransformation3D Scale
        {
            get;
        }

        /// <summary>
        /// Event that is raised when <see cref="Scale" /> has changed.
        /// </summary>
        event EventHandler<PropertyChangedEventArgs<ScaleTransformation3D>> ScaleChanged;

        #endregion

        #region [====== Resize ======]        

        /// <summary>
        /// Resizes the current object equally along all three dimensions.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="scaleFactor"/> is <c>0</c> or less than <c>0</c>.
        /// </exception>
        void Resize(float scaleFactor);

        /// <summary>
        /// Resizes the current object equally along all three dimensions.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        void Resize(ScaleFactor scaleFactor);

        /// <summary>
        /// Resizes the current object using the specified scale factors.
        /// </summary>
        /// <param name="x">Scale-factor along the X-axis.</param>
        /// <param name="y">Scale-factor along the Y-axis.</param>
        /// <param name="z">Scale-factor along the Z-axis.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="x"/>, <paramref name="y"/> or <paramref name="z"/> is <c>0</c> or less than <c>0</c>.
        /// </exception>
        void Resize(float x, float y, float z);

        /// <summary>
        /// Resizes the current object using the specified scale factors.
        /// </summary>
        /// <param name="x">Scale-factor along the X-axis.</param>
        /// <param name="y">Scale-factor along the Y-axis.</param>
        /// <param name="z">Scale-factor along the Z-axis.</param>
        void Resize(ScaleFactor x, ScaleFactor y, ScaleFactor z);       

        /// <summary>
        /// Resizes the current object exactly to the specified scale and equally along all three dimensions.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="scaleFactor"/> is <c>0</c> or less than <c>0</c>.
        /// </exception>
        void ResizeTo(float scaleFactor);

        /// <summary>
        /// Resizes the current object exactly to the specified scale equally along all three dimensions.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        void ResizeTo(ScaleFactor scaleFactor);

        /// <summary>
        /// Resizes the current object exactly to the specified scales.
        /// </summary>
        /// <param name="x">Scale-factor along the X-axis.</param>
        /// <param name="y">Scale-factor along the Y-axis.</param>
        /// <param name="z">Scale-factor along the Z-axis.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Either <paramref name="x"/>, <paramref name="y"/> or <paramref name="z"/> is <c>0</c> or less than <c>0</c>.
        /// </exception>
        void ResizeTo(float x, float y, float z);

        /// <summary>
        /// Resizes the current object exactly to the specified scales.
        /// </summary>
        /// <param name="x">Scale-factor along the X-axis.</param>
        /// <param name="y">Scale-factor along the Y-axis.</param>
        /// <param name="z">Scale-factor along the Z-axis.</param>
        void ResizeTo(ScaleFactor x, ScaleFactor y, ScaleFactor z);

        #endregion
    }
}
