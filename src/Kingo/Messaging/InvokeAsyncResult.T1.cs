namespace Kingo.Messaging
{
    /// <summary>
    /// Represents the result of a message handler or query invocation in the pipeline.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class InvokeAsyncResult<TResult>
    {
        internal InvokeAsyncResult(TResult value)
        {
            Value = value;
        }

        /// <summary>
        /// Value of the result.
        /// </summary>
        public TResult Value
        {
            get;
        }
    }
}
