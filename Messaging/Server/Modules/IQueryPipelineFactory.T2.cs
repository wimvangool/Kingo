namespace System.ComponentModel.Server.Modules
{
    /// /// <summary>
    /// When implemented by a class, represents a factory that can be used to build a <see cref="IQuery{TMessageIn, TMessageOut}" /> pipeline.
    /// </summary>    
    /// <typeparam name="TMessageIn">Type of the messages that go into the pipeline.</typeparam>
    /// <typeparam name="TMessageOut">Type of the messages that come out of the pipeline.</typeparam>
    public interface IQueryPipelineFactory<TMessageIn, TMessageOut> where TMessageIn : class        
    {
        /// <summary>
        /// Creates and returns a pipeline of <see cref="IQuery{TMessageIn, TMessageOut}">Queries</see>
        /// on top of the specified <paramref name="query"/>.
        /// </summary>
        /// <param name="query">The query on which the pipeline is built.</param>
        /// <returns>The created pipeline</returns>.
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is <c>null</c>.
        /// </exception>
        IQuery<TMessageIn, TMessageOut> CreateQueryPipeline(IQuery<TMessageIn, TMessageOut> query);
    }
}
