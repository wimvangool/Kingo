using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingo.Constraints
{
    internal sealed class OrConstraint : CompositeConstraint
    {
        public OrConstraint(params IConstraint[] constraints) :
            base(constraints) { }

        public OrConstraint(IEnumerable<IConstraint> constraints) :
            base(constraints) { }

        #region [====== Logical Operations ======]

        /// <inheritdoc />
        public override IConstraint Or(IConstraint constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            if (constraint == this)
            {
                return this;
            }
            return new OrConstraint(Constraints.Concat(new [] { constraint }));
        }

        #endregion

        #region [====== IsValid ======]

        public override bool IsValid(object value) =>
            Constraints.Any(constraint => constraint.IsValid(value));

        #endregion
    }
}
