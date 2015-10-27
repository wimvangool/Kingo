using System;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{TMessage}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== Apply ======]

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, TValueOut> Apply<TMessage, TValueOut>(this IMemberConstraint<TMessage, TValueOut> member, IConstraint<TValueOut> constraint)
        {
            return EnsureNotNull(member).Satisfies(constraint);
        }

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, TValueOut> Apply<TMessage, TValueOut>(this IMemberConstraint<TMessage, TValueOut> member, Func<TMessage, IConstraint<TValueOut>> constraintFactory)
        {
            return EnsureNotNull(member).Satisfies(constraintFactory);
        }

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraint">The constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, TOther> Apply<TMessage, TValueOut, TOther>(this IMemberConstraint<TMessage, TValueOut> member, IConstraint<TValueOut, TOther> constraint, Func<string, string> nameSelector = null)
        {
            return EnsureNotNull(member).Satisfies(constraint, nameSelector);
        }

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<TMessage, TOther> Apply<TMessage, TValueOut, TOther>(this IMemberConstraint<TMessage, TValueOut> member, Func<TMessage, IConstraint<TValueOut, TOther>> constraintFactory, Func<string, string> nameSelector = null)
        {
            return EnsureNotNull(member).Satisfies(constraintFactory, nameSelector);
        }

        private static TMember EnsureNotNull<TMember>(TMember member) where TMember : class
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member;
        }

        #endregion        
    }
}
