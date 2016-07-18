using System;
using System.Windows.Threading;

namespace Kingo.SharpDX
{
    /// <summary>
    /// When implemented by a class, represents the rendering pipeline of an image.
    /// </summary>
    public interface IDirectXRenderingPipeline : IDisposable
    {
        #region [====== Dispatcher & Name ======]

        /// <summary>
        /// Dispatcher of the pipeline that can be used to send messages to the rendering loop.
        /// </summary>
        Dispatcher Dispatcher
        {
            get;
        }

        /// <summary>
        /// Name of the pipeline.
        /// </summary>
        string Name
        {
            get;
        }

        #endregion

        #region [====== FramesPerSecond ======]

        /// <summary>
        /// The number of frames rendered per second.
        /// </summary>
        int FramesPerSecond
        {
            get;
        }

        /// <summary>
        /// The time it takes for each frame to render.
        /// </summary>
        TimeSpan TimePerFrame
        {
            get;
        }

        #endregion

        #region [====== RenderNextFrame ======]        

        /// <summary>
        /// Renders the next frame.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The pipeline has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The pipeline has not yet been initialized.
        /// </exception>
        void RenderNextFrame();

        #endregion
    }
}
