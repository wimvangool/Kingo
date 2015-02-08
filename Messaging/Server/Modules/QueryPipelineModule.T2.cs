namespace System.ComponentModel.Server.Modules
{
    /// <summary>
    /// Serves as a base class for modules that are part of a <see cref="IQuery{TMessageIn, TMessageOut}" />-pipeline.
    /// </summary>
    public abstract class QueryPipelineModule<TMessageIn, TMessageOut> : Query<TMessageIn, TMessageOut> where TMessageIn : class
    {
        /// <summary>
        /// The next <see cref="IQuery{TMessageIn, TMessageOut}" /> to invoke.
        /// </summary>
        protected abstract IQuery<TMessageIn, TMessageOut> Query
        {
            get;
        }                
    }
}
