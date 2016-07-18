using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base class for any <see cref="IDirectXRenderingPipeline"  /> implementation.
    /// </summary>
    public abstract class DirectXRenderingPipeline : DirectXRenderingPipelineBase, IDirectXRenderingPipeline
    {
        private const int _SamplesPerSecond = 5;
        private const double _TimerIntervalInMilliseconds = 1000.0 / _SamplesPerSecond;        

        private readonly Timer _framesPerSecondTimer;
        private int _framesPerSecond;      
        private int _frameCount;       

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXRenderingPipeline" /> class.
        /// </summary>
        protected DirectXRenderingPipeline()
        {
            _framesPerSecondTimer = new Timer(_TimerIntervalInMilliseconds);
            _framesPerSecondTimer.AutoReset = true;
            _framesPerSecondTimer.Elapsed += HandleTimerElapsed;            
        }

        #region [====== Dispatcher & Name ======]

        /// <inheritdoc />
        public override string Name
        {
            get { return GetType().Name; }
        }

        #endregion

        #region [====== FramesPerSecond & TimePerFrame ======]

        /// <inheritdoc />
        public override int FramesPerSecond
        {
            get { return _framesPerSecond; }
        }

        /// <inheritdoc />
        public override TimeSpan TimePerFrame
        {
            get { return FramesPerSecond == 0 ? TimeSpan.MaxValue : ToTimePerFrame(FramesPerSecond); }
        }

        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _framesPerSecond = Interlocked.Exchange(ref _frameCount, 0) * _SamplesPerSecond;
        }

        private static TimeSpan ToTimePerFrame(int framesPerSecond)
        {            
            var secondsPerFrame = 1.0 / framesPerSecond;
            var ticksPerFrame = TimeSpan.TicksPerSecond * secondsPerFrame;
            
            return TimeSpan.FromTicks((long) ticksPerFrame);
        }        

        #endregion

        #region [====== RenderNextFrame ======]        

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            _framesPerSecondTimer.Start();            
        }

        /// <inheritdoc />
        protected override void RenderFrame()
        {
            Interlocked.Increment(ref _frameCount);
        }

        #endregion

        #region [====== Dispose ======]

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            _framesPerSecondTimer.Dispose();

            base.DisposeManagedResources();
        }

        #endregion
    }
}
