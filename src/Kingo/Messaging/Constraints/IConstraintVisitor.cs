using System;
using System.Collections.Generic;

namespace Kingo.Messaging.Constraints
{
    /// <summary>
    /// When implemented by a class, represent a visitor of <see cref="IConstraint">Constraints</see>.
    /// </summary>
    public interface IConstraintVisitor
    {
        /// <summary>
        /// Visits the specified <paramref name="andConstraint"/> and its children.
        /// </summary>
        /// <param name="andConstraint">A logical AND-constraint.</param>
        /// <param name="childConstraints">The children of the AND-constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="andConstraint"/> or <paramref name="childConstraints"/> is <c>null</c>.
        /// </exception>
        void VisitAnd(IConstraint andConstraint, IEnumerable<IConstraint> childConstraints);

        /// <summary>
        /// Visits the specified <paramref name="orConstraint"/> and its children.
        /// </summary>
        /// <param name="orConstraint">A logical OR-constraint.</param>
        /// <param name="childConstraints">The children of the OR-constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="orConstraint"/> or <paramref name="childConstraints"/> is <c>null</c>.
        /// </exception>
        void VisitOr(IConstraintWithErrorMessage orConstraint, IEnumerable<IConstraint> childConstraints);

        /// <summary>
        /// Visits the specified <paramref name="inverseConstraint"/> and the corresponding <paramref name="invertedConstraint"/>.
        /// </summary>
        /// <param name="inverseConstraint">The logical NOT- or inverse constraint.</param>
        /// <param name="invertedConstraint">The inverted constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inverseConstraint"/> or <paramref name="invertedConstraint"/> is <c>null</c>.
        /// </exception>
        void VisitInverse(IConstraintWithErrorMessage inverseConstraint, IConstraint invertedConstraint);

        /// <summary>
        /// Visits the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">A non-composite constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        void Visit(IConstraintWithErrorMessage constraint);
    }
}
