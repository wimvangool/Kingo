using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Constraints
{
    internal sealed class AndConstraint : CompositeConstraint
    {
        public AndConstraint(params IConstraint[] constraints) :
            base(constraints) { }

        public AndConstraint(IEnumerable<IConstraint> constraints) :
            base(constraints) { }

        private AndConstraint(IEnumerable<IConstraint> constraints, params IConstraint[] addedConstraints) :
            base(constraints.Concat(addedConstraints)) { }

        #region [====== Logical Operations ======]

        /// <inheritdoc />
        public override IConstraint And(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            if (constraint == this)
            {
                return constraint;
            }
            return new AndConstraint(Constraints, constraint);
        }

        #endregion

        #region [====== IsValid ======]

        public override bool IsValid(object value) =>
            Constraints.All(constraint => constraint.IsValid(value));

        #endregion
    }
}
