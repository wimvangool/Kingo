using System.Threading;
using Kingo.Reflection;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented, represents an operation of a <see cref="IMicroProcessor" />.
    /// </summary>
    public abstract class MicroProcessorOperation : IMicroProcessorOperation
    {
        /// <inheritdoc />
        public abstract IMessage Message
        {
            get;
        }

        /// <inheritdoc />
        public abstract CancellationToken Token
        {
            get;
        }

        /// <inheritdoc />
        public abstract MicroProcessorOperationType Type
        {
            get;
        }

        /// <inheritdoc />
        public abstract MicroProcessorOperationKind Kind
        {
            get;
        }

        /// <inheritdoc />
        public override string ToString() =>
            ToString(GetType().FriendlyName());

        internal string ToString(object operationOrMethod) =>
            $"{operationOrMethod} [{Kind}]";
    }
}
