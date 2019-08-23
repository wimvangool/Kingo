using System;

namespace Kingo.MicroServices
{
    /// <summary>
    /// When implemented by a class, represents the result of verifying whether or not a request is not valid.
    /// </summary>
    public interface IIsNotValidResult
    {
        /// <summary>
        /// Verifies all validation-errors using the specified <paramref name="errorValidator" />.
        /// </summary>
        /// <param name="errorValidator">
        /// The validator that will be used to validate all validation errors.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorValidator"/> is <c>null</c>.
        /// </exception>
        void And(Action<IValidationErrorCollection> errorValidator);
    }
}
