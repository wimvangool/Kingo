using System;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base class for all classes that implement the <see cref="IDirectXRenderingComponent{T}"/> interface.
    /// </summary>
    /// <typeparam name="TPipeline">Type of the pipeline this component belongs to.</typeparam>
    public abstract class DirectXRenderingComponent<TPipeline> : Disposable, IDirectXRenderingComponent<TPipeline> where TPipeline : class
    {
        private bool _isInitialized;

        #region [====== RenderNextFrame ======]       

        /// <summary>
        /// Initializes the component.
        /// </summary>
        /// <param name="pipeline">The rendering pipeline.</param>
        protected virtual void Initialize(TPipeline pipeline)
        {
            _isInitialized = true;
        }

        /// <inheritdoc />
        public void RenderNextFrame(TPipeline pipeline)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }
            if (_isInitialized)
            {
                RenderFrame(pipeline);                               
            }
            else
            {
                Initialize(pipeline);
                RenderFrame(pipeline);
            }            
        }

        /// <summary>
        /// Renders the next frame.
        /// </summary>
        /// <param name="pipeline">The current rendering pipeline.</param>
        protected virtual void RenderFrame(TPipeline pipeline) { }               

        #endregion
    }
}
