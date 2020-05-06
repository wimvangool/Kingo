using System;

namespace Kingo.MicroServices.Configuration
{
    /// <summary>
    /// When implemented by a class, represents a builder that can be used to configure
    /// the <see cref="IMicroProcessor" /> to use in your service.
    /// </summary>
    public interface IMicroProcessorBuilder : IMicroProcessorOptions
    {
        /// <summary>
        /// Returns the collection of all message-handlers that are to be used by the processor.
        /// </summary>
        MessageHandlerCollection MessageHandlers
        {
            get;
        }

        /// <summary>
        /// Returns the collection of queries that are to be used by the processor.
        /// </summary>
        QueryCollection Queries
        {
            get;
        }

        /// <summary>
        /// Returns the collection of all repositories that are to be used by the processor.
        /// </summary>
        RepositoryCollection Repositories
        {
            get;
        }

        /// <summary>
        /// Adds the specified <paramref name="components"/> to be registered and used by the <see cref="IMicroProcessor" />.
        /// </summary>
        /// <param name="components">The components to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="components"/> is <c>null</c>.
        /// </exception>
        void Add(MicroProcessorComponentCollection components);
    }
}
