using System;

namespace Kingo.SharpDX
{
    /// <summary>
    /// When implemented by a concrete class, represents a component that is part of the rendering process of an image.
    /// </summary>
    /// <typeparam name="TPipeline">Type of the pipeline this component belongs to.</typeparam>
    public interface IDirectXRenderingComponent<in TPipeline> : IDisposable where TPipeline : class
    {        
        /// <summary>
        /// Renders the next frame.
        /// </summary>
        /// <param name="pipeline">The current rendering pipeline.</param>
        /// <exception cref="ObjectDisposedException">
        /// This component has already been disposed.
        /// </exception>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pipeline"/> is <c>null</c>.
        /// </exception>
        void RenderNextFrame(TPipeline pipeline);
    }
}
