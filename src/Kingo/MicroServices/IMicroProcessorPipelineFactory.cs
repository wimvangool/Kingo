using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents a factory for building a message handler or query pipeline on top
    /// of a message handler or query that is about to be invoked.
    /// </summary>
    public interface IMicroProcessorPipelineFactory
    {
        /// <summary>
        /// Creates and returns a message handler pipeline on top of the specified <paramref name="handler"/>.
        /// </summary>
        /// <param name="handler">The handler that represents the end of the pipeline.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        MessageHandler CreatePipeline(MessageHandler handler);

        /// <summary>
        /// Creates and returns a query pipeline on top of the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query that represents the end of the pipeline.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        Query<TMessageOut> CreatePipeline<TMessageOut>(Query<TMessageOut> query);
    }
}
