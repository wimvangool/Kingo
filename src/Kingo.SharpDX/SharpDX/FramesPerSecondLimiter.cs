using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Represents a pipeline decorator that limits the maximum frames per second to about 60.
    /// </summary>
    public sealed class FramesPerSecondLimiter : DirectXRenderingPipelineDecorator
    {
        private const int _MaximumWaitTimeInMilliseconds = 15;
        private readonly Timer _limiter;
        private readonly AutoResetEvent _limiterHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramesPerSecondLimiter" /> class.
        /// </summary>
        /// <param name="pipeline">The decorated pipeline.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pipeline"/> is <c>null</c>.
        /// </exception>
        public FramesPerSecondLimiter(IDirectXRenderingPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }            
            Pipeline = pipeline;

            _limiter = new Timer(15.0);
            _limiter.AutoReset = true;
            _limiter.Elapsed += HandleLimiterElapsed;

            _limiterHandle = new AutoResetEvent(true);
        }

        /// <inheritdoc />
        protected override IDirectXRenderingPipeline Pipeline
        {
            get;
        }        

        #region [====== Initialize & RenderNextFrame ======]

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            _limiter.Start();
        }

        /// <inheritdoc />
        protected override void RenderFrame()
        {
            // The current thread should only wait a fixed amount of time before
            // continuing, so that the image can still respond to user input
            // in a timely fashion.
            if (_limiterHandle.WaitOne(_MaximumWaitTimeInMilliseconds))
            {
                base.RenderFrame();
            }
        }

        private void HandleLimiterElapsed(object sender, ElapsedEventArgs e)
        {
            _limiterHandle.Set();
        }

        #endregion

        #region [====== Dispose ======]

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            _limiterHandle.Dispose();
            _limiter.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
