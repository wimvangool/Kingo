namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a message handler or query invocation in the pipeline.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class InvokeAsyncResult<TResult>
    {
        /// <summary>
        /// Obtains the value of this result.
        /// </summary>
        public abstract TResult GetValue();
    }
}
