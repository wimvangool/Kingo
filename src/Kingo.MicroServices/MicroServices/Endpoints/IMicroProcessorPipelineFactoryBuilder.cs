using System;

namespace Kingo.MicroServices.Endpoints
{
    /// <summary>
    /// When implemented by a class, represents a builder of a <see cref="IMicroProcessorPipelineFactory"/>, which
    /// on its turn can create a message handler or query pipeline on top of a specified message handler or query.
    /// </summary>
    public interface IMicroProcessorPipelineFactoryBuilder
    {
        /// <summary>
        /// Adds the specified <paramref name="filter"/> to the pipeline.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="filter"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="filter"/> specifies an invalid stage.
        /// </exception>
        void Add(IMicroProcessorFilter filter);

        /// <summary>
        /// Removes the specified <typeparamref name="TAttribute"/> from the pipeline.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute to remove from the pipeline.</typeparam>
        void Remove<TAttribute>() where TAttribute : Attribute, IMicroProcessorFilter;

        /// <summary>
        /// Removes all attributes declared on message handlers and queries from the pipeline.
        /// </summary>
        void RemoveAllAttributes();

        /// <summary>
        /// Build and returns a new pipeline factory.
        /// </summary>
        /// <returns>A new <see cref="IMicroProcessorPipelineFactory"/>.</returns>
        IMicroProcessorPipelineFactory Build();
    }
}
