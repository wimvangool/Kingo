using System;
using System.Linq;

namespace Kingo.MicroServices
{
    /// <summary>
    /// Contains extension methods for instances of type <see cref="IMicroProcessorOperationContext"/>.
    /// </summary>
    public static class MicroProcessorOperationContextExtensions
    {
        #region [====== BadRequestExceptions ======]

        /// <summary>
        /// Creates and returns a new <see cref="BadRequestException"/> with the specified <paramref name="message"/> and
        /// <paramref name="innerException"/> and a stack-trace obtained from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An operation context.</param>
        /// <param name="message">Optional message of the exception.</param>
        /// <param name="innerException">Optional inner-exception.</param>
        /// <returns>A new <see cref="BadRequestException"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static BadRequestException NewBadRequestException(this IMicroProcessorOperationContext context, string message = null, Exception innerException = null) =>
            new BadRequestException(CaptureOperationStackTrace(context), message, innerException);

        /// <summary>
        /// Creates and returns a new <see cref="UnauthorizedRequestException"/> with the specified <paramref name="message"/> and
        /// <paramref name="innerException"/> and a stack-trace obtained from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An operation context.</param>
        /// <param name="message">Optional message of the exception.</param>
        /// <param name="innerException">Optional inner-exception.</param>
        /// <returns>A new <see cref="UnauthorizedRequestException"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static UnauthorizedRequestException NewUnauthorizedRequestException(this IMicroProcessorOperationContext context, string message = null, Exception innerException = null) =>
            new UnauthorizedRequestException(CaptureOperationStackTrace(context), message, innerException);

        /// <summary>
        /// Creates and returns a new <see cref="NotFoundException"/> with the specified <paramref name="message"/> and
        /// <paramref name="innerException"/> and a stack-trace obtained from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An operation context.</param>
        /// <param name="message">Optional message of the exception.</param>
        /// <param name="innerException">Optional inner-exception.</param>
        /// <returns>A new <see cref="NotFoundException"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static NotFoundException NewNotFoundException(this IMicroProcessorOperationContext context, string message = null, Exception innerException = null) =>
            new NotFoundException(CaptureOperationStackTrace(context), message, innerException);

        /// <summary>
        /// Creates and returns a new <see cref="ConflictException"/> with the specified <paramref name="message"/> and
        /// <paramref name="innerException"/> and a stack-trace obtained from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An operation context.</param>
        /// <param name="message">Optional message of the exception.</param>
        /// <param name="innerException">Optional inner-exception.</param>
        /// <returns>A new <see cref="ConflictException"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static ConflictException NewConflictException(this IMicroProcessorOperationContext context, string message = null, Exception innerException = null) =>
            new ConflictException(CaptureOperationStackTrace(context), message, innerException);

        #endregion

        #region [====== InternalServerErrorException ======]

        /// <summary>
        /// Creates and returns a new <see cref="InternalServerErrorException"/> with the specified <paramref name="message"/> and
        /// <paramref name="innerException"/> and a stack-trace obtained from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An operation context.</param>
        /// <param name="message">Optional message of the exception.</param>
        /// <param name="innerException">Optional inner-exception.</param>
        /// <returns>A new <see cref="InternalServerErrorException"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static InternalServerErrorException NewInternalServerErrorException(this IMicroProcessorOperationContext context, string message = null, Exception innerException = null) =>
            new InternalServerErrorException(CaptureOperationStackTrace(context), message, innerException);

        /// <summary>
        /// Creates and returns a new <see cref="GatewayTimeoutException"/> with the specified <paramref name="message"/> and
        /// <paramref name="innerException"/> and a stack-trace obtained from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">An operation context.</param>
        /// <param name="message">Optional message of the exception.</param>
        /// <param name="innerException">Optional inner-exception.</param>
        /// <returns>A new <see cref="GatewayTimeoutException"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static GatewayTimeoutException NewGatewayTimeoutException(this IMicroProcessorOperationContext context, string message = null, Exception innerException = null) =>
            new GatewayTimeoutException(CaptureOperationStackTrace(context), message, innerException);

        #endregion

        #region [====== CaptureOperationStackTrace ======]

        /// <summary>
        /// Captures and returns the <see cref="MicroProcessorOperationStackTrace" /> that represents the current stack
        /// of operations traced by the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">A context.</param>
        /// <returns>The <see cref="MicroProcessorOperationStackTrace" /> that represents the current stack of operations</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is <c>null</c>.
        /// </exception>
        public static MicroProcessorOperationStackTrace CaptureOperationStackTrace(this IMicroProcessorOperationContext context) =>
            CaptureOperationStackTrace(NotNull(context).StackTrace);

        private static MicroProcessorOperationStackTrace CaptureOperationStackTrace(IAsyncMethodOperationStackTrace stackTrace) =>
            new MicroProcessorOperationStackTrace(stackTrace.Select(ToMicroProcessorOperationStackItem));

        private static MicroProcessorOperationStackItem ToMicroProcessorOperationStackItem(IAsyncMethodOperation operation) =>
            new MicroProcessorOperationStackItem(operation.Method.ComponentType, operation.Message);

        #endregion

        private static IMicroProcessorOperationContext NotNull(IMicroProcessorOperationContext context) =>
            context ?? throw new ArgumentNullException(nameof(context));
    }
}
