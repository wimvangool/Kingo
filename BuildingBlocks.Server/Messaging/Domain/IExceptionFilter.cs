using System;

namespace Kingo.BuildingBlocks.Messaging.Domain
{
    /// <summary>
    /// When implemented by a class, represents a filter of <see cref="DomainException">DomainExceptions</see>
    /// that are selectively converted into <see cref="FunctionalException">FunctionalExceptions</see> to indicate
    /// that the exception was caused by an invalid request.
    /// </summary>
    public interface IDomainExceptionFilter
    {
        /// <summary>
        /// Attempts to convert the specified <paramref name="domainException"/> to a <see cref="FunctionalException" />.
        /// </summary>
        /// <param name="message">The message that was being handled when the exception was thrown.</param>
        /// <param name="domainException">The exception to convert.</param>
        /// <param name="functionalException">
        /// If this method returns <c>true</c>, this parameter will refer to the converted exception;
        /// otherwise, it will be <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if the conversion was succesful; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> or <paramref name="domainException"/> is <c>null</c>.
        /// </exception>
        bool TryConvertToFunctionalException(IMessage message, DomainException domainException, out FunctionalException functionalException);
    }
}
