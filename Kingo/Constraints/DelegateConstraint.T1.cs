using System;
using Kingo.Messaging;

namespace Kingo.Constraints
{
    /// <summary>
    /// Represents a constraint that checks whether or not a value satisfies a predicate that is passed in as a delegate.
    /// </summary>
    public sealed class DelegateConstraint<TValue> : Constraint<TValue>
    {
        private readonly Func<TValue, bool> _constraint;
        private readonly object _errorMessageArgument;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">A delegate that represents the constraint.</param> 
        /// <param name="errorMessageArgument">
        /// The object that is used to format the error message on behalf of this constraint.
        /// </param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraint(Func<TValue, bool> constraint, object errorMessageArgument = null)          
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
            _errorMessageArgument = errorMessageArgument;
        }

        private DelegateConstraint(DelegateConstraint<TValue> constraint, StringTemplate errorMessage) 
            : base(constraint, errorMessage)
        {
            _constraint = constraint._constraint;
            _errorMessageArgument = constraint._errorMessageArgument;
        }

        private DelegateConstraint(DelegateConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _constraint = constraint._constraint;
            _errorMessageArgument = constraint._errorMessageArgument;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new DelegateConstraint<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new DelegateConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== Conversion ======]

        /// <inheritdoc />
        public override Func<TValue, bool> ToDelegate()
        {
            return _constraint;
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return _constraint.Invoke(value);
        }

        /// <inheritdoc />
        public override bool IsNotSatisfiedBy(TValue value, out IErrorMessageBuilder errorMessage)
        {
            if (base.IsNotSatisfiedBy(value, out errorMessage))
            {
                if (_errorMessageArgument != null)
                {
                    errorMessage.Put(Name, _errorMessageArgument);
                }
                return true;
            }
            return false;
        }

        #endregion
    }
}
