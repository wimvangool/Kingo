using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Kingo.Resources;

namespace Kingo.SharpDX
{
    /// <summary>
    /// When implemented by a concrete class, represents an image that is directly rendered by DirectX.
    /// </summary>
    public abstract class DirectXImage : Disposable
    {        
        private readonly Disposable<Form> _control;
        private DirectXRenderLoop _renderLoop;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectXImage" /> class.
        /// </summary>
        protected DirectXImage()
        {
            _control = new Disposable<Form>(CreateForm);                                                
        }

        internal Form Control
        {
            get { return _control.Value; }
        }

        /// <summary>
        /// Creates and returns a new <see cref="Form"/> that is used as a render-target for the image.
        /// </summary>
        /// <returns>A new <see cref="Form"/>.</returns>
        protected virtual Form CreateForm()
        {
            return new Form()
            {
                TopLevel = false,
                ControlBox = false,
                ShowInTaskbar = false,
                ShowIcon = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };            
        }

        /// <inheritdoc />
        protected override void DisposeManagedResources()
        {
            _control.Dispose();

            base.DisposeManagedResources();
        }

        #region [====== Rendering ======]

        internal event EventHandler<RenderingStartedEventArgs> RenderingStarted;

        private void OnRenderingStarted(Control control)
        {            
            RenderingStarted.Raise(this, new RenderingStartedEventArgs(control));
        }

        internal event EventHandler RenderingStopped;

        private void OnRenderingStopped()
        {
            RenderingStopped.Raise(this);         
        }

        internal void StartRendering()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_renderLoop == null)
            {
                Control.Show();

                _renderLoop = new DirectXRenderLoop(this);
                _renderLoop.Start();

                OnRenderingStarted(Control);
            }
            else
            {
                throw NewAlreadyRenderingException();
            }
        }

        internal void StopRendering()
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_renderLoop == null)
            {
                throw NewAlreadyIdleException();
            }
            Control.Hide();

            _renderLoop.Stop();
            _renderLoop.Dispose();
            _renderLoop = null;

            OnRenderingStopped();
        }

        private static Exception NewAlreadyRenderingException()
        {
            return new InvalidOperationException(ExceptionMessages.DirectXImage_AlreadyRendering);
        }

        private static Exception NewAlreadyIdleException()
        {
            return new InvalidOperationException(ExceptionMessages.DirectXImage_AlreadyNonRendering);
        }

        /// <summary>
        /// Creates and returns a new 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="handle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        protected internal abstract IDirectXRenderingPipeline CreateRenderingPipeline(Dispatcher dispatcher, IntPtr handle, Size size);

        #endregion
    }
}
