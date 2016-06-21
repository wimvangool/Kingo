using System;
using System.Windows.Threading;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base class for all classes that represent a decorator on an existing <see cref="IDirectXRenderingPipeline" /> implementation.
    /// </summary>
    public abstract class DirectXRenderingPipelineDecorator : Disposable, IDirectXRenderingPipeline
    {
        /// <summary>
        /// De decorated pipeline.
        /// </summary>
        protected abstract IDirectXRenderingPipeline Pipeline
        {
            get;
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            Pipeline.Dispose();

            base.DisposeManagedResources();
        }        

        #region [====== Dispatcher & Name ======]

        /// <inheritdoc />
        public Dispatcher Dispatcher
        {
            get { return Pipeline.Dispatcher; }
        }

        /// <inheritdoc />
        public virtual string Name
        {
            get { return Pipeline.Name; }
        }

        #endregion

        #region [====== FramesPerSecond ======]

        /// <inheritdoc />
        public int FramesPerSecond
        {
            get { return Pipeline.FramesPerSecond; }
        }

        /// <inheritdoc />
        public TimeSpan TimePerFrame
        {
            get { return Pipeline.TimePerFrame; }
        }

        #endregion

        #region [====== Initialize & RenderNextFrame ======]

        /// <inheritdoc />
        public virtual void Initialize()
        {
            Pipeline.Initialize();
        }

        /// <inheritdoc />
        public virtual void RenderNextFrame()
        {
            Pipeline.RenderNextFrame();
        }

        #endregion
    }
}
