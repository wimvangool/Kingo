using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents the argument of ...
    /// </summary>
    public sealed class MemberConstraintEventArgs : EventArgs
    {
        /// <summary>
        /// The member constraint that was added or removed.
        /// </summary>
        public readonly IMemberConstraint MemberConstraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintEventArgs" /> class.
        /// </summary>
        /// <param name="memberConstraint">The member constraint that was added or removed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberConstraint"/> is <c>null</c>.
        /// </exception>
        public MemberConstraintEventArgs(IMemberConstraint memberConstraint)
        {
            if (memberConstraint == null)
            {
                throw new ArgumentNullException("memberConstraint");
            }
            MemberConstraint = memberConstraint;
        }
    }
}
