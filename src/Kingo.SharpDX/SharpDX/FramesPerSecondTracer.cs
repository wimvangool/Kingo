using System;
using System.Diagnostics;
using System.Timers;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Represents a pipeline decorator that traces the FPS of a pipeline every second.
    /// </summary>
    public sealed class FramesPerSecondTracer : DirectXRenderingPipelineDecorator
    {
        private const double _OneSecond = 1000.0;        
        private readonly Timer _framesPerSecondTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramesPerSecondTracer" /> class.
        /// </summary>
        /// <param name="pipeline">The decorated pipeline.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pipeline"/> is <c>null</c>.
        /// </exception>
        public FramesPerSecondTracer(IDirectXRenderingPipeline pipeline)
        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }
            Pipeline = pipeline;

            _framesPerSecondTimer = new Timer(_OneSecond);
            _framesPerSecondTimer.AutoReset = true;
            _framesPerSecondTimer.Elapsed += HandleSecondElapsed;
        }

        /// <inheritdoc />
        protected override IDirectXRenderingPipeline Pipeline
        {
            get;
        }

        #region [====== RenderNextFrame ======]

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            _framesPerSecondTimer.Start();
        }

        private void HandleSecondElapsed(object sender, ElapsedEventArgs e)
        {
            TraceLine($"FPS = {FramesPerSecond} ({TimePerFrame.TotalMilliseconds.ToString("F2")} ms per frame)");
        }

        private void TraceLine(string message)
        {
            Trace.WriteLine($"[{Name}] {message}.");
        }

        #endregion

        #region [====== Dispose ======]

        protected override void DisposeManagedResources()
        {
            _framesPerSecondTimer.Dispose();

            Pipeline.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
