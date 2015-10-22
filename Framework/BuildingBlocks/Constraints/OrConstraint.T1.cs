using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.BuildingBlocks.Constraints
{
    internal sealed class OrConstraint<TValue> : Constraint<TValue>
    {
        private readonly IConstraint<TValue>[] _constraints;

        internal OrConstraint(IConstraint<TValue> left, IConstraint<TValue> constraint)
            : base(null, null)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraints = new [] { left, constraint };
        }

        internal OrConstraint(IEnumerable<IConstraint<TValue>> constraints, StringTemplate errorMessage, Identifier name)
            : base(errorMessage, name)
        {
            _constraints = constraints.ToArray();
        }

        private OrConstraint(OrConstraint<TValue> left, IConstraint<TValue> constraint)
            : base(left.ErrorMessage, left.Name)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraints = left._constraints.Add(constraint);
        } 

        private OrConstraint(OrConstraint<TValue> constraint, Identifier name)
            : base(constraint.ErrorMessage, name)
        {
            _constraints = constraint._constraints;
        }

        private OrConstraint(OrConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(errorMessage, constraint.Name)
        {
            _constraints = constraint._constraints;
        }               

        #region [====== Name & ErrorMessage ======]

        public override IConstraintWithErrorMessage<TValue> WithName(Identifier name)
        {
            return new OrConstraint<TValue>(this, name);
        }

        public override IConstraintWithErrorMessage<TValue> WithErrorMessage(StringTemplate errorMessage)
        {
            return new OrConstraint<TValue>(this, errorMessage);
        }

        #endregion

        #region [====== And, Or & Invert ======]

        public override IConstraintWithErrorMessage<TValue> Or(IConstraint<TValue> constraint)
        {            
            return new OrConstraint<TValue>(this, constraint);
        }

        #endregion

        #region [====== IsSatisfiedBy & IsNotSatisfiedBy ======]

        public override bool IsSatisfiedBy(TValue value)
        {
            return _constraints.Any(constraint => constraint.IsSatisfiedBy(value));
        }

        public override bool IsNotSatisfiedBy(TValue value, out IErrorMessage errorMessage)
        {
            // To obtain the error message for an OR-constraint, we first check all child-constraints.
            // While checking, we collect all of their failed constraints through their error messages.
            // If any of them passes, the OR-constraint passes as a whole, so in that case we break the
            // loop immediately. Note that the OR-constraint always has two or more child constraints
            // to check, so the for-loop is never skipped immediately to return an empty set of failed
            // constraints.
            var failedConstraints = new IConstraintWithErrorMessage[_constraints.Length];

            for (int index = 0; index < _constraints.Length; index++)
            {
                var constraint = _constraints[index];
                IErrorMessage childErrorMessage;

                if (constraint.IsNotSatisfiedBy(value, out childErrorMessage))
                {
                    failedConstraints[index] = childErrorMessage.FailedConstraint;
                    continue;
                }
                errorMessage = null;
                return false;
            }
            errorMessage = new FailedOrConstraintMessage(this, failedConstraints);
            return true;
        }        

        #endregion
    }
}
