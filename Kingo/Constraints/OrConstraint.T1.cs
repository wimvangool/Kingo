using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Constraints
{
    internal sealed class OrConstraint<TValue> : Constraint<TValue>
    {
        private readonly IConstraint<TValue>[] _constraints;

        internal OrConstraint(IConstraint<TValue> left, IConstraint<TValue> constraint)           
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraints = new [] { left, constraint };
        }

        internal OrConstraint(IEnumerable<IConstraint<TValue>> constraints)           
        {
            _constraints = constraints.ToArray();
        }

        private OrConstraint(OrConstraint<TValue> left, IConstraint<TValue> constraint)
            : base(left)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _constraints = left._constraints.Add(constraint);
        } 

        private OrConstraint(OrConstraint<TValue> constraint, Identifier name)
            : base(constraint, name)
        {
            _constraints = constraint._constraints;
        }

        private OrConstraint(OrConstraint<TValue> constraint, StringTemplate errorMessage)
            : base(constraint, errorMessage)
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

        #region [====== Visitor ======]
        
        public override void AcceptVisitor(IConstraintVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("visitor");
            }
            visitor.VisitOr(this, _constraints);
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

        #endregion
    }
}
