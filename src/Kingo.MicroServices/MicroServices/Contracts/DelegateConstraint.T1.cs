using System;
using System.ComponentModel.DataAnnotations;

namespace Kingo.MicroServices.Contracts
{
    /// <summary>
    /// Represents a constraint that is implemented through a delegate.
    /// </summary>
    /// <typeparam name="TValue">Type of value that can be validated by this constraint.</typeparam>
    public sealed class DelegateConstraint<TValue> : Constraint<TValue>
    {
        private readonly Func<TValue, ValidationContext, ValidationResult> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">
        /// Delegate that implements the validation logic.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraint(Func<TValue, ValidationContext, ValidationResult> constraint)
        {
            _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
        }

        /// <inheritdoc />
        public override ValidationResult IsValid(TValue value, ValidationContext validationContext) =>
            _constraint.Invoke(value, validationContext);
    }
}
