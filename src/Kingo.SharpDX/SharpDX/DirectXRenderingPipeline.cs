using System;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using Kingo.Resources;
using Timer = System.Timers.Timer;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base class for any <see cref="IDirectXRenderingPipeline"  /> implementation.
    /// </summary>
    public abstract class DirectXRenderingPipeline : Disposable, IDirectXRenderingPipeline
    {
        private const int _SamplesPerSecond = 5;
        private const double _TimerIntervalInMilliseconds = 1000.0 / _SamplesPerSecond;        

        private readonly Timer _framesPerSecondTimer;        
        private int _frameCount;
        private bool _isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXRenderingPipeline" /> class.
        /// </summary>
        protected DirectXRenderingPipeline()
        {
            _framesPerSecondTimer = new Timer(_TimerIntervalInMilliseconds);
            _framesPerSecondTimer.AutoReset = true;
            _framesPerSecondTimer.Elapsed += HandleTimerElapsed;            
        }

        /// <inheritdoc />
        public abstract Dispatcher Dispatcher
        {
            get;
        }

        /// <inheritdoc />
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        #region [====== Dispose ======]

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            _framesPerSecondTimer.Dispose();

            base.DisposeManagedResources();
        }        

        #endregion

        #region [====== FramesPerSecond ======]

        /// <inheritdoc />
        public int FramesPerSecond
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public TimeSpan TimePerFrame
        {
            get { return FramesPerSecond == 0 ? TimeSpan.MaxValue : ToTimePerFrame(FramesPerSecond); }
        }

        private void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            FramesPerSecond = Interlocked.Exchange(ref _frameCount, 0) * _SamplesPerSecond;
        }

        private static TimeSpan ToTimePerFrame(int framesPerSecond)
        {            
            var secondsPerFrame = 1.0 / framesPerSecond;
            var ticksPerFrame = TimeSpan.TicksPerSecond * secondsPerFrame;
            
            return TimeSpan.FromTicks((long) ticksPerFrame);
        }        

        #endregion

        #region [====== Initialize & RenderNextFrame ======]

        void IDirectXRenderingPipeline.Initialize()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isInitialized)
            {
                throw NewAlreadyInitializedException(this);
            }
            Initialize();            
        }

        /// <summary>
        /// Initializes the pipeline.
        /// </summary>
        protected virtual void Initialize()
        {
            _framesPerSecondTimer.Start();
            _isInitialized = true;
        }

        void IDirectXRenderingPipeline.RenderNextFrame()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isInitialized)
            {                
                RenderNextFrame();
                return;
            }
            throw NewNotInitializedException(this);
        }

        /// <summary>
        /// Renders the next frame.
        /// </summary>
        protected virtual void RenderNextFrame()
        {
            Interlocked.Increment(ref _frameCount);
        }

        private static Exception NewAlreadyInitializedException(IDirectXRenderingPipeline pipeline)
        {
            var messageFormat = ExceptionMessages.DirectXRenderingComponent_AlreadyInitialized;
            var message = string.Format(messageFormat, pipeline.Name);
            return new InvalidOperationException(message);
        }

        private static Exception NewNotInitializedException(IDirectXRenderingPipeline pipeline)
        {
            var messageFormat = ExceptionMessages.DirectXRenderingComponent_NotInitialized;
            var message = string.Format(messageFormat, pipeline.Name);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
