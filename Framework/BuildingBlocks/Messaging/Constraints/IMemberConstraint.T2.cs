﻿using System;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint for a specific member of a message.
    /// </summary>    
    /// <typeparam name="TMessage">Type of the message the error messages are produced for.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public interface IMemberConstraint<TMessage, TResult> : IMemberConstraint<TMessage>
    {
        /// <summary>
        /// Descends one level down in the validation-hierarchy.
        /// </summary>
        /// <param name="innerConstraintFactory">
        /// The delegate that is used to define constraints on the properties or children of this member's value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerConstraintFactory"/> is <c>null</c>.
        /// </exception>
        void And(Action<IMemberConstraintSet<TResult>> innerConstraintFactory);

        /// <summary>
        /// Verifies that this member's value is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>        
        IMemberConstraint<TMessage, TResult> IsNotInstanceOf<TOther>(string errorMessage = null);

        /// <summary>
        /// Verifies that this member'value is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageConsumer" /> when verification fails.
        /// </param>
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>        
        IMemberConstraint<TMessage, TOther> IsInstanceOf<TOther>(string errorMessage = null);

        /// <summary>
        /// Applies the specified <paramref name="constraint"/> in addition to the current constraint(s) and returns
        /// a <see cref="IMemberConstraint{TMessage}" /> of which the value has been converted using the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TOther> Satisfies<TOther>(IConstraintWithErrorMessage<TMessage, TResult, TOther> constraint, Func<string, string> nameSelector = null);
    }
}
