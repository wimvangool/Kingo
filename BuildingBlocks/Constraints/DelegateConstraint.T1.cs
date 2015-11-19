using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a constraint that checks whether or not a value satisfies a predicate that is passed in as a delegate.
    /// </summary>
    public sealed class DelegateConstraint<TValue> : Constraint<TValue>
    {
        private readonly Func<TValue, bool> _constraint;       

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{T}" /> class.
        /// </summary>
        /// <param name="constraint">A delegate that represents the constraint.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraint(Func<TValue, bool> constraint)          
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
        }

        private DelegateConstraint(DelegateConstraint<TValue> constraint, StringTemplate errorMessage) 
            : base(constraint, errorMessage)
        {
            _constraint = constraint._constraint;
        }

        private DelegateConstraint(DelegateConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _constraint = constraint._constraint;
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

        #endregion
    }
}
