namespace Kingo.MicroServices
{
    /// <summary>
    /// Represents the result of a message handler or query invocation in the pipeline.
    /// </summary>
    /// <typeparam name="TValue">Type of the result.</typeparam>
    public interface IMicroProcessorOperationResult<out TValue>
    {
        /// <summary>
        /// Obtains the value of this result.
        /// </summary>
        TValue Value
        {
            get;
        }
    }
}
