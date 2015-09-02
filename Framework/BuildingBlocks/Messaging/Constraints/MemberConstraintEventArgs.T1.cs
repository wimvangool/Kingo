using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents the argument of the <see cref="MemberConstraintSet{T}.ConstraintAdded" /> and
    /// <see cref="MemberConstraintSet{T}.ConstraintAdded" /> events.
    /// </summary>
    /// <typeparam name="T">Type of the object the error messages are produced for.</typeparam>
    public sealed class MemberConstraintEventArgs<T> : EventArgs
    {
        /// <summary>
        /// The member constraint that was added or removed.
        /// </summary>
        public readonly IMemberConstraint<T> MemberConstraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConstraintEventArgs{T}" /> class.
        /// </summary>
        /// <param name="memberConstraint">The member constraint that was added or removed.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberConstraint"/> is <c>null</c>.
        /// </exception>
        public MemberConstraintEventArgs(IMemberConstraint<T> memberConstraint)
        {
            if (memberConstraint == null)
            {
                throw new ArgumentNullException("memberConstraint");
            }
            MemberConstraint = memberConstraint;
        }
    }
}
