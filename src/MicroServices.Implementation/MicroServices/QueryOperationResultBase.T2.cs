using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Serves as a base-class for results that are returned by queries.
    /// </summary>
    /// <typeparam name="TInput">Type of the input-message.</typeparam>
    /// <typeparam name="TOutput">Type of the output-message.</typeparam>
    public abstract class QueryOperationResultBase<TInput, TOutput>
        where TInput : class, IMessage
        where TOutput : class, IMessage
    {
        /// <summary>
        /// The request-message (input) of the query.
        /// </summary>
        public abstract TInput Input
        {
            get;
        }

        /// <summary>
        /// The response message (output) of the query.
        /// </summary>
        public abstract TOutput Output
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{Input.Content.GetType().FriendlyName()} --> {Output.Content.GetType().FriendlyName()}";
    }
}
