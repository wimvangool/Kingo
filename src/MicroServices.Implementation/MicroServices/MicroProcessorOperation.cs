using System;
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
        public abstract IMessageToProcess Message
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

        #region [====== Exception Handling ======]

        internal static InternalServerErrorException NewInternalServerErrorException(Exception exception, MicroProcessorOperationStackTrace operationStackTrace) =>
            new InternalServerErrorException(exception.Message, exception, operationStackTrace);

        internal static GatewayTimeoutException NewGatewayTimeoutException(Exception exception, MicroProcessorOperationStackTrace operationStackTrace) =>
            new GatewayTimeoutException(ExceptionMessages.MicroProcessorOperation_GatewayTimeout, exception, operationStackTrace);

        #endregion
    }
}
