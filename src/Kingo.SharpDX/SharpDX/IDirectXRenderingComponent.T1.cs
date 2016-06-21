using System;

namespace Kingo.SharpDX
{
    /// <summary>
    /// When implemented by a concrete class, represents a component that is part of the rendering process of an image.
    /// </summary>
    /// <typeparam name="TContext">Type of the context to render to.</typeparam>
    public interface IDirectXRenderingComponent<in TContext> : IDisposable where TContext : class
    {
        /// <summary>
        /// Initializes the component.
        /// </summary>
        /// <param name="context">The DirectX Context.</param>
        /// <exception cref="ObjectDisposedException">
        /// This component has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This component has aready been initialized.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        void Initialize(TContext context);

        /// <summary>
        /// Renders the next frame.
        /// </summary>
        /// <param name="context">The DirectX Context.</param>
        /// <exception cref="ObjectDisposedException">
        /// This component has already been disposed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This component has not yet been initialized.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        void RenderNextFrame(TContext context);
    }
}
