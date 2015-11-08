using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static partial class BasicConstraints
    {
        #region [====== Select ======]

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TMember"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>        
        /// <param name="member">A member.</param>
        /// <param name="memberExpression">The expression that selects the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="memberExpression"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, TMember> Select<T, TValueOut, TMember>(this IMemberConstraint<T, TValueOut> member, Expression<Func<TValueOut, TMember>> memberExpression)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.And(memberExpression);
        }

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TMember"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>     
        /// <param name="member">A member.</param>   
        /// <param name="memberDelegate">The delegate that selects the member.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>,<paramref name="memberDelegate"/> or <paramref name="memberName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="memberName"/> is not a valid identifier.
        /// </exception>
        public static IMemberConstraint<T, TMember> Select<T, TValueOut, TMember>(this IMemberConstraint<T, TValueOut> member, Func<TValueOut, TMember> memberDelegate, string memberName)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.And(memberDelegate, memberName);
        }

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TMember"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>      
        /// <param name="member">A member.</param>  
        /// <param name="memberDelegate">The delegate that selects the member.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/>, <paramref name="memberDelegate"/> or <paramref name="memberName" /> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, TMember> Select<T, TValueOut, TMember>(this IMemberConstraint<T, TValueOut> member, Func<TValueOut, TMember> memberDelegate, Identifier memberName)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            return member.And(memberDelegate, memberName);
        }

        #endregion

        #region [====== Apply ======]

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraint{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, TValueOut> Apply<T, TValueOut>(this IMemberConstraint<T, TValueOut> member, IConstraint<TValueOut> constraint)
        {
            return EnsureNotNull(member).Satisfies(constraint);
        }

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraint{T}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, TValueOut> Apply<T, TValueOut>(this IMemberConstraint<T, TValueOut> member, Func<T, IConstraint<TValueOut>> constraintFactory)
        {
            return EnsureNotNull(member).Satisfies(constraintFactory);
        }

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraint">The constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, TOther> Apply<T, TValueOut, TOther>(this IMemberConstraint<T, TValueOut> member, IFilter<TValueOut, TOther> constraint, Func<string, string> nameSelector = null)
        {
            return EnsureNotNull(member).Satisfies(constraint, nameSelector);
        }

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="member">The member to apply the constraint to.</param>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{T}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> or <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        public static IMemberConstraint<T, TOther> Apply<T, TValueOut, TOther>(this IMemberConstraint<T, TValueOut> member, Func<T, IFilter<TValueOut, TOther>> constraintFactory, Func<string, string> nameSelector = null)
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
