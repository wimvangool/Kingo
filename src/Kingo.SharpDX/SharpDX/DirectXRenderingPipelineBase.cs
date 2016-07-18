using System;
using System.Windows.Threading;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base-class implementation of the <see cref="IDirectXRenderingPipeline"/> interface.
    /// </summary>
    public abstract class DirectXRenderingPipelineBase : Disposable, IDirectXRenderingPipeline
    {
        private bool _isInitialized;

        internal DirectXRenderingPipelineBase() { }

        #region [====== Dispatcher & Name ======]

        /// <inheritdoc />
        public abstract Dispatcher Dispatcher
        {
            get;
        }

        /// <inheritdoc />
        public abstract string Name
        {
            get;
        }

        #endregion

        #region [====== FramesPerSecond & TimePerFrame ======]

        /// <inheritdoc />
        public abstract int FramesPerSecond
        {
            get;
        }

        /// <inheritdoc />
        public abstract TimeSpan TimePerFrame
        {
            get;
        }

        #endregion

        #region [====== RenderNextFrame ======]

        /// <summary>
        /// Initializes the pipeline.
        /// </summary>
        protected virtual void Initialize()
        {
            _isInitialized = true;
        }

        /// <inheritdoc />
        public void RenderNextFrame()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isInitialized)
            {
                RenderFrame();
            }
            else
            {
                Initialize();
                RenderFrame();
            }
        }

        /// <summary>
        /// Renders the next frame.
        /// </summary>
        protected abstract void RenderFrame();

        #endregion
    }
}
