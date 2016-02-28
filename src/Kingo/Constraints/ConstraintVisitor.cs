using System;
using System.Collections.Generic;

namespace Kingo.Constraints
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IConstraintVisitor"/> interface.
    /// </summary>
    public abstract class ConstraintVisitor : IConstraintVisitor
    {        
        void IConstraintVisitor.VisitAnd(IConstraint andConstraint, IEnumerable<IConstraint> childConstraints)
        {
            if (andConstraint == null)
            {
                throw new ArgumentNullException(nameof(andConstraint));
            }
            if (childConstraints == null)
            {
                throw new ArgumentNullException(nameof(childConstraints));
            }
            VisitAnd(andConstraint);

            foreach (var constraint in childConstraints)
            {
                constraint.AcceptVisitor(this);
            }
        }

        /// <summary>
        /// Visits the specified <paramref name="andConstraint"/>.
        /// </summary>
        /// <param name="andConstraint">A logical AND-constraint.</param>
        protected abstract void VisitAnd(IConstraint andConstraint);

        void IConstraintVisitor.VisitOr(IConstraintWithErrorMessage orConstraint, IEnumerable<IConstraint> childConstraints)
        {
            if (orConstraint == null)
            {
                throw new ArgumentNullException(nameof(orConstraint));
            }
            if (childConstraints == null)
            {
                throw new ArgumentNullException(nameof(childConstraints));
            }
            VisitOr(orConstraint);

            foreach (var constraint in childConstraints)
            {
                constraint.AcceptVisitor(this);
            }
        }

        /// <summary>
        /// Visits the specified <paramref name="orConstraint"/>.
        /// </summary>
        /// <param name="orConstraint">A logical OR-constraint.</param>
        protected abstract void VisitOr(IConstraintWithErrorMessage orConstraint);
        
        void IConstraintVisitor.VisitInverse(IConstraintWithErrorMessage inverseConstraint, IConstraint invertedConstraint)
        {
            if (inverseConstraint == null)
            {
                throw new ArgumentNullException(nameof(inverseConstraint));
            }
            if (invertedConstraint == null)
            {
                throw new ArgumentNullException(nameof(invertedConstraint));
            }
            VisitInverse(inverseConstraint);

            invertedConstraint.AcceptVisitor(this);
        }

        /// <summary>
        /// Visits the specified <paramref name="inverseConstraint"/>.
        /// </summary>
        /// <param name="inverseConstraint">A logical NOT-constraint.</param>
        protected abstract void VisitInverse(IConstraintWithErrorMessage inverseConstraint);

        void IConstraintVisitor.Visit(IConstraintWithErrorMessage constraint)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            Visit(constraint);
        }

        /// <summary>
        /// Visits the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The visited constraint.</param>
        protected abstract void Visit(IConstraintWithErrorMessage constraint);        
    }
}
