using System;
using Kingo.Resources;

namespace Kingo.SharpDX
{
    /// <summary>
    /// Serves as a base class for all classes that implement the <see cref="IDirectXRenderingComponent{T}"/> interface.
    /// </summary>
    /// <typeparam name="TContext">Type of the context to render to.</typeparam>
    public abstract class DirectXRenderingComponent<TContext> : Disposable, IDirectXRenderingComponent<TContext> where TContext : class
    {
        private bool _isInitialized;

        #region [====== Initialize & RenderNextFrame ======]

        void IDirectXRenderingComponent<TContext>.Initialize(TContext context)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isInitialized)
            {
                throw NewAlreadyInitializedException(this);
            }
            Initialize(context);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        /// <param name="context">The DirectX Context.</param>
        protected virtual void Initialize(TContext context)
        {
            _isInitialized = true;
        }

        void IDirectXRenderingComponent<TContext>.RenderNextFrame(TContext context)
        {
            if (IsDisposed)
            {
                throw NewObjectDisposedException();
            }
            if (_isInitialized)
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }
                RenderNextFrame(context);
                return;
            }            
            throw NewNotInitializedException(this);
        }

        /// <summary>
        /// Performs rendering operations for the next frame.
        /// </summary>
        /// <param name="context">The DirectX Context.</param>
        protected virtual void RenderNextFrame(TContext context) { }

        private static Exception NewAlreadyInitializedException(object component)
        {
            var messageFormat = ExceptionMessages.DirectXRenderingComponent_AlreadyInitialized;
            var message = string.Format(messageFormat, component.GetType().Name);
            return new InvalidOperationException(message);
        }

        private static Exception NewNotInitializedException(object component)
        {
            var messageFormat = ExceptionMessages.DirectXRenderingComponent_NotInitialized;
            var message = string.Format(messageFormat, component.GetType().Name);
            return new InvalidOperationException(message);
        }

        #endregion
    }
}
