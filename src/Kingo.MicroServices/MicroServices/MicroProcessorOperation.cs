using System.Threading;

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
        public abstract MicroProcessorOperationKinds Kind
        {
            get;
        }
    }
}
