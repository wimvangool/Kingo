using System;
using System.Linq.Expressions;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Represents a constraint for a specific member of a message.
    /// </summary>    
    /// <typeparam name="TMessage">Type of the message the error messages are produced for.</typeparam>
    /// <typeparam name="TValueOut">Type of the result the value is converted to.</typeparam>
    public interface IMemberConstraint<TMessage, TValueOut> : IMemberConstraint<TMessage>
    {
        #region [====== And ======]

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TMember"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>
        /// <typeparam name="TMember">Type of the member.</typeparam>
        /// <param name="fieldOrPropertyExpression">The expression that selects the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrPropertyExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyExpression"/> is not a supported expression.
        /// </exception>
        IMemberConstraint<TMessage, TMember> And<TMember>(Expression<Func<TValueOut, TMember>> fieldOrPropertyExpression);

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TMember"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>
        /// <typeparam name="TMember">Type of the member.</typeparam>
        /// <param name="fieldOrProperty">The delegate that selects the member.</param>
        /// <param name="fieldOrPropertyName">Name of the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyName"/> is not a valid identifier.
        /// </exception>
        IMemberConstraint<TMessage, TMember> And<TMember>(Func<TValueOut, TMember> fieldOrProperty, string fieldOrPropertyName);

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TMember"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>
        /// <typeparam name="TMember">Type of the member.</typeparam>
        /// <param name="fieldOrProperty">The delegate that selects the member.</param>
        /// <param name="fieldOrPropertyName">Name of the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName" /> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TMember> And<TMember>(Func<TValueOut, TMember> fieldOrProperty, Identifier fieldOrPropertyName);

        /// <summary>
        /// Descends one level down in the validation-hierarchy.
        /// </summary>
        /// <param name="innerConstraintFactory">
        /// The delegate that is used to define constraints on the properties or children of this member's value.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="innerConstraintFactory"/> is <c>null</c>.
        /// </exception>
        void And(Action<IMemberConstraintSet<TValueOut>> innerConstraintFactory);

        #endregion

        #region [====== IsInstanceOf, IsNotInstanceOf & As ======]

        /// <summary>
        /// Verifies that this member's value is not an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>This member.</returns>        
        IMemberConstraint<TMessage, TValueOut> IsNotInstanceOf<TOther>(string errorMessage = null);

        /// <summary>
        /// Verifies that this member'value is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>        
        IMemberConstraint<TMessage, TOther> IsInstanceOf<TOther>(string errorMessage = null);

        /// <summary>
        /// Casts the output of this member to an instance of the specified type <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to cast this member's type to.</typeparam>
        /// <returns>The casted member.</returns>
        IMemberConstraint<TMessage, TOther> As<TOther>() where TOther : class;

        #endregion

        #region [====== Satisfies ======]

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>   
        /// <param name="errorMessage">Error message of the constraint.</param>             
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TValueOut> Satisfies(Func<TValueOut, bool> constraint, string errorMessage = null);

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TValueOut> Satisfies(IConstraint<TValueOut> constraint);

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TValueOut> Satisfies(Func<TMessage, IConstraint<TValueOut>> constraintFactory);
        
        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TOther> Satisfies<TOther>(IConstraint<TValueOut, TOther> constraint, Func<string, string> nameSelector = null);

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>        
        /// <param name="nameSelector">Optional delegate used to convert the current member's name to a new name.</param>
        /// <returns>A <see cref="IMemberConstraint{TMessage}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        IMemberConstraint<TMessage, TOther> Satisfies<TOther>(Func<TMessage, IConstraint<TValueOut, TOther>> constraintFactory, Func<string, string> nameSelector = null);

        #endregion
    }
}
