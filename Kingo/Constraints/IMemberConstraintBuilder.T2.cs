using System;
using System.Linq.Expressions;
using Kingo.Messaging;

namespace Kingo.Constraints
{
    /// <summary>
    /// Represents a constraint for a specific member of a message.
    /// </summary>    
    /// <typeparam name="T">Type of the message the error messages are produced for.</typeparam>
    /// <typeparam name="TValueOut">Type of the result the value is converted to.</typeparam>
    public interface IMemberConstraintBuilder<T, TValueOut> : IMemberConstraintBuilder<T>
    {
        #region [====== And ======]

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TOther"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>
        /// <typeparam name="TOther">Type of the member.</typeparam>
        /// <param name="fieldOrProperty">The expression that selects the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrProperty"/> is not a supported expression.
        /// </exception>
        IMemberConstraintBuilder<T, TOther> And<TOther>(Expression<Func<T, TValueOut, TOther>> fieldOrProperty);

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TOther"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>
        /// <typeparam name="TOther">Type of the member.</typeparam>
        /// <param name="fieldOrProperty">The delegate that selects the member.</param>
        /// <param name="fieldOrPropertyName">Name of the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fieldOrPropertyName"/> is not a valid identifier.
        /// </exception>
        IMemberConstraintBuilder<T, TOther> And<TOther>(Func<T, TValueOut, TOther> fieldOrProperty, string fieldOrPropertyName);

        /// <summary>
        /// Selects a field or property of type <typeparamref name="TOther"/> from the current value of type <typeparamref name="TValueOut"/>
        /// with the intention to add some field- or property-specific constraints.
        /// </summary>
        /// <typeparam name="TOther">Type of the member.</typeparam>
        /// <param name="fieldOrProperty">The delegate that selects the member.</param>
        /// <param name="fieldOrPropertyName">Name of the member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fieldOrProperty"/> or <paramref name="fieldOrPropertyName" /> is <c>null</c>.
        /// </exception>
        IMemberConstraintBuilder<T, TOther> And<TOther>(Func<T, TValueOut, TOther> fieldOrProperty, Identifier fieldOrPropertyName);

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
        IMemberConstraintBuilder<T, TValueOut> IsNotInstanceOf<TOther>(string errorMessage = null);

        /// <summary>
        /// Verifies that this member'value is an instance of <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to compare this member's type to.</typeparam>
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>
        /// <returns>A member casted to <typeparamref name="TOther"/>.</returns>        
        IMemberConstraintBuilder<T, TOther> IsInstanceOf<TOther>(string errorMessage = null);

        /// <summary>
        /// Casts the output of this member to an instance of the specified type <typeparamref name="TOther"/>.
        /// </summary>
        /// <typeparam name="TOther">Type to cast this member's type to.</typeparam>
        /// <returns>The casted member.</returns>
        IMemberConstraintBuilder<T, TOther> As<TOther>() where TOther : class;

        #endregion

        #region [====== HasItem ======]

        /// <summary>
        /// Verifies that this member has an item at the specified <paramref name="index"/> and returns it.
        /// </summary>               
        /// <param name="index">Index of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem>(int index, string errorMessage = null);

        /// <summary>
        /// Verifies that this member has an item at the specified <paramref name="index"/> and returns it.
        /// </summary>               
        /// <param name="index">Index of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem>(string index, string errorMessage = null);

        /// <summary>
        /// Verifies that this member has an item at the specified <paramref name="index"/> and returns it.
        /// </summary>               
        /// <param name="index">Index of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(TIndex index, string errorMessage = null);

        /// <summary>
        /// Verifies that this member has an item at the specified indices and returns it.
        /// </summary>              
        /// <param name="indexA">First index of the element to select.</param>      
        /// <param name="indexB">Second index of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>              
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(TIndexA indexA, TIndexB indexB, string errorMessage = null);

        /// <summary>
        /// Verifies that this member has an item at the specified <paramref name="indexFactory"/> and returns it.
        /// </summary>               
        /// <param name="indexFactory">Index of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>  
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indexFactory"/> is <c>null</c>.
        /// </exception>            
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndex>(Func<T, TIndex> indexFactory, string errorMessage = null);

        /// <summary>
        /// Verifies that this member has an item at the specified indices and returns it.
        /// </summary>              
        /// <param name="indexAFactory">First index of the element to select.</param>      
        /// <param name="indexBFactory">Second index of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns> 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indexAFactory" /> or <paramref name="indexBFactory" /> is <c>null</c>.
        /// </exception>               
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem, TIndexA, TIndexB>(Func<T, TIndexA> indexAFactory, Func<T, TIndexB> indexBFactory, string errorMessage = null);

        /// <summary>
        /// Verifies that this member has an item at the indicies that are provided by the specified <paramref name="indexListFactory"/>.
        /// </summary>               
        /// <param name="indexListFactory">A list of delegates that return the indices of the element to select.</param>      
        /// <param name="errorMessage">
        /// The error message that is added to a <see cref="IErrorMessageReader" /> when verification fails.
        /// </param>     
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> instance that contains the member's value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="indexListFactory"/> is <c>null</c>.
        /// </exception>       
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IMemberConstraintBuilder<T, TItem> HasItem<TItem>(IndexListFactory<T> indexListFactory, string errorMessage = null);               

        #endregion

        #region [====== Satisfies ======]

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>   
        /// <param name="errorMessage">Error message of the constraint.</param>  
        /// <param name="errorMessageArgument">
        /// The object that is used to format the error message on behalf of this constraint.
        /// </param>               
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraintBuilder<T, TValueOut> Satisfies(Predicate<TValueOut> constraint, string errorMessage = null, object errorMessageArgument = null);        

        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraintBuilder<T, TValueOut> Satisfies(IConstraint<TValueOut> constraint);

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        IMemberConstraintBuilder<T, TValueOut> Satisfies(Func<T, IConstraint<TValueOut>> constraintFactory);
        
        /// <summary>
        /// Applies the specified <paramref name="constraint"/>.
        /// </summary>
        /// <param name="constraint">The constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraint"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(IFilter<TValueOut, TOther> constraint);

        /// <summary>
        /// Applies the constraint that is created by the specified <paramref name="constraintFactory"/>.
        /// </summary>
        /// <param name="constraintFactory">A delegate used to create the constraint to apply.</param>                
        /// <returns>A <see cref="IMemberConstraintBuilder{T}" /> that has applied the specified <paramref name="constraintFactory"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraintFactory"/> is <c>null</c>.
        /// </exception>
        IMemberConstraintBuilder<T, TOther> Satisfies<TOther>(Func<T, IFilter<TValueOut, TOther>> constraintFactory);

        #endregion
    }
}
