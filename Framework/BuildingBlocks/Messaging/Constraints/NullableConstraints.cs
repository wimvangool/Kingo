﻿using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Contains a set of extension methods specific for members of type <see cref="IMemberConstraint{T}" />.
    /// </summary>
    public static class NullableConstraints
    {        
        #region [====== HasValue ======]

        /// <summary>
        /// Verifies whether or not the <paramref name="member"/>'s value is not <c>null</c>.
        /// </summary>
        /// <param name="member">A member.</param> 
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>       
        /// <returns>A member containing the value of this member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="member"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IMemberConstraint<T, TValue> HasValue<T, TValue>(this IMemberConstraint<T, TValue?> member, string errorMessage = null) where TValue : struct
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }            
            return member.Satisfies(HasValueConstraint<TValue>(errorMessage));
        }

        /// <summary>
        /// Creates and returns a new <see cref="IConstraintWithErrorMessage{T, S}" /> that checks whether or not a nullable has a value.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="errorMessage">Error message to return when the member fails.</param>
        /// <returns>A new <see cref="IConstraintWithErrorMessage{T, S}" />.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue?, TValue> HasValueConstraint<TValue>(string errorMessage = null) where TValue : struct
        {
            return New.Constraint<TValue?, TValue>(value => value.HasValue, value => value.Value)
                .WithDisplayFormat("{member.Name}.HasValue")
                .WithErrorFormat(errorMessage ?? ConstraintErrors.NullableConstraints_IsNotNull)
                .BuildConstraint();
        }

        #endregion
    }
}
