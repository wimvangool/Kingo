using System;
using System.Windows.Threading;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base class for all classes that represent a decorator on an existing <see cref="IDirectXRenderingPipeline" /> implementation.
    /// </summary>
    public abstract class DirectXRenderingPipelineDecorator : DirectXRenderingPipelineBase
    {        
        /// <summary>
        /// De decorated pipeline.
        /// </summary>
        protected abstract IDirectXRenderingPipeline Pipeline
        {
            get;
        }        

        #region [====== Dispatcher & Name ======]

        /// <inheritdoc />
        public override Dispatcher Dispatcher
        {
            get { return Pipeline.Dispatcher; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return Pipeline.Name; }
        }

        #endregion

        #region [====== FramesPerSecond & TimePerFrame ======]

        /// <inheritdoc />
        public override int FramesPerSecond
        {
            get { return Pipeline.FramesPerSecond; }
        }

        /// <inheritdoc />
        public override TimeSpan TimePerFrame
        {
            get { return Pipeline.TimePerFrame; }
        }

        #endregion

        #region [====== RenderNextFrame ======]        

        /// <inheritdoc />
        protected override void RenderFrame()
        {
            Pipeline.RenderNextFrame();
        }

        #endregion

        #region [====== Dispose ======]

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            Pipeline.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
