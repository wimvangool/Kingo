using System;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraintBuilder{T}" />.
    /// </summary>
    public static partial class BasicConstraints
    {        
        #region [====== Apply ======]

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValueOut> Apply<T, TValueOut>(this IMemberConstraintBuilder<T, TValueOut> member, IConstraint<TValueOut> constraint) =>
            EnsureNotNull(member).Satisfies(constraint);

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraintBuilder<T, TValueOut> Apply<T, TValueOut>(this IMemberConstraintBuilder<T, TValueOut> member, Func<T, IConstraint<TValueOut>> constraintFactory) =>
            EnsureNotNull(member).Satisfies(constraintFactory);

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraintBuilder<T, TOther> Apply<T, TValueOut, TOther>(this IMemberConstraintBuilder<T, TValueOut> member, IFilter<TValueOut, TOther> constraint) =>
            EnsureNotNull(member).Satisfies(constraint);

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraintBuilder<T, TOther> Apply<T, TValueOut, TOther>(this IMemberConstraintBuilder<T, TValueOut> member, Func<T, IFilter<TValueOut, TOther>> constraintFactory) =>
            EnsureNotNull(member).Satisfies(constraintFactory);

        private static TMember EnsureNotNull<TMember>(TMember member) where TMember : class
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            return member;
        }

        #endregion        
    }
}
