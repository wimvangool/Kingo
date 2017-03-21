using System;

namespace Kingo.Messaging.Validation.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a constraint.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Accepts the specified <paramref name="visitor"/>.
        /// </summary>
        /// <param name="visitor">The visitor of this constraint.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="visitor"/> is <c>null</c>.
        /// </exception>
        void AcceptVisitor(IConstraintVisitor visitor);
    }
}
