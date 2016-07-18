using System;
using System.Threading;
using System.Windows.Threading;

namespace Kingo.SharpDX
{    
    internal sealed class DirectXRenderLoop : Disposable
    {
        private readonly DirectXImage _image;
        private readonly Thread _renderThread;
        private readonly Action _renderNextFrame;

        private readonly ManualResetEventSlim _imageDispatcherRunning;
        private Dispatcher _imageDispatcher;
        private IDirectXRenderingPipeline _renderingPipeline;

        public DirectXRenderLoop(DirectXImage image)
        {
            _image = image;
            _imageDispatcherRunning = new ManualResetEventSlim();

            _renderThread = new Thread(Run);
            _renderThread.SetApartmentState(ApartmentState.STA);
            _renderThread.IsBackground = true;
            _renderThread.Name = $"RenderThread ({_image.GetType().Name})";
            _renderNextFrame = RenderNextFrame;
        }

        protected override void DisposeManagedResources()
        {
            if (IsRunning)
            {
                Stop();
            }
            _imageDispatcherRunning.Dispose();

            base.DisposeManagedResources();
        }        

        private bool IsRunning
        {
            get { return _imageDispatcher != null; }
        }

        public void Start()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            _renderThread.Start(new { _image.Control.Handle, _image.Control.Size });

            _imageDispatcherRunning.Wait();
            _imageDispatcher.BeginInvoke(DispatcherPriority.Render, _renderNextFrame);
        }

        private void Run(dynamic arguments)
        {
            _imageDispatcher = Dispatcher.CurrentDispatcher;
            _imageDispatcherRunning.Set();

            _renderingPipeline = _image.CreateRenderingPipeline(_imageDispatcher, arguments.Handle, arguments.Size);            

            try
            {
                // This will block the current thread until the Dispatcher is shut down.
                Dispatcher.Run();
            }
            finally
            {
                _renderingPipeline.Dispose();
                _renderingPipeline = null;
            }
        }

        private void RenderNextFrame()
        {
            // Each time a frame must be rendered, we immediately re-schedule another rendering cycle.
            _imageDispatcher.BeginInvoke(DispatcherPriority.Render, _renderNextFrame);
            _renderingPipeline.RenderNextFrame();
        }

        public void Stop()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            _imageDispatcherRunning.Wait();
            _imageDispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            _imageDispatcher = null;
        }
    }
}
