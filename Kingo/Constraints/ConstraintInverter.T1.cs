using System;

namespace Kingo.Constraints
{
    /// <summary>
    /// Represents a constraint that negates another constraint.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    public sealed class ConstraintInverter<TValue> : Constraint<TValue>
    {
        private readonly IConstraintWithErrorMessage<TValue> _constraint;
        private readonly StringTemplate _errorMessage;
        private readonly Identifier _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintInverter{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint that is to be negated.</param>  
        /// <param name="errorMessage">Error message to use when no other is specified.</param>      
        /// <param name="name">Name to use when no other is specified.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format or <paramref name="name"/> is not a valid identifier.
        /// </exception>
        public ConstraintInverter(IConstraintWithErrorMessage<TValue> constraint, string errorMessage, string name = null)
            : this(constraint, StringTemplate.ParseOrNull(errorMessage), Identifier.ParseOrNull(name)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintInverter{T}" /> class.
        /// </summary>
        /// <param name="constraint">The constraint that is to be negated.</param>  
        /// <param name="errorMessage">Error message to use when no other is specified.</param>      
        /// <param name="name">Name to use when no other is specified.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public ConstraintInverter(IConstraintWithErrorMessage<TValue> constraint, StringTemplate errorMessage = null, Identifier name = null)            
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraint = constraint;
            _errorMessage = errorMessage;
            _name = name;
        }

        private ConstraintInverter(ConstraintInverter<TValue> constraint, StringTemplate errorMessage) 
            : base(constraint, errorMessage)
        {
            _constraint = constraint._constraint;
            _errorMessage = constraint._errorMessage;
            _name = constraint._name;
        }

        private ConstraintInverter(ConstraintInverter<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _constraint = constraint._constraint;
            _errorMessage = constraint._errorMessage;
            _name = constraint._name;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        protected override Identifier NameIfNotSpecified
        {
            get { return _name ?? base.NameIfNotSpecified; }
        }

        /// <inheritdoc />
        protected override StringTemplate ErrorMessageIfNotSpecified
        {
            get { return _errorMessage ?? base.ErrorMessageIfNotSpecified; }
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new ConstraintInverter<TValue>(this, name);
        }

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new ConstraintInverter<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== Visitor ======]

        /// <inheritdoc />
        public override void AcceptVisitor(IConstraintVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }
            visitor.VisitInverse(this, _constraint);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        /// <inheritdoc />
        public override IConstraintWithErrorMessage<TValue> Invert(StringTemplate errorMessage, Identifier name = null)
        {
            return _constraint
                .WithErrorMessage(errorMessage ?? _constraint.ErrorMessage)
                .WithName(name ?? _constraint.Name);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value)
        {
            return !_constraint.IsSatisfiedBy(value);
        }

        /// <inheritdoc />
        public override bool IsNotSatisfiedBy(TValue value, out IErrorMessageBuilder errorMessage)
        {
            if (_constraint.IsSatisfiedBy(value))
            {
                errorMessage = Constraints.ErrorMessageBuilder.Build(this, value);
                return true;
            }
            errorMessage = null;
            return false;
        }

        #endregion
    }
}
