using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// This class contains factory methods to create objects purely for syntactical reasons.
    /// </summary>
    public static class New
    {
        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static DelegateConstraintBuilder<TValue> Constraint<TValue>(Func<TValue, bool> constraint)
        {
            return new DelegateConstraintBuilder<TValue>(constraint);
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>        
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>
        /// <param name="valueConverter">Delegate used to convert the value of type <typeparamref name="TValue"/> to a value of type <typeparamref name="TResult"/>.</param>       
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="valueConverter"/> is <c>null</c>.
        /// </exception>        
        public static DelegateConstraintBuilder<TValue, TResult> Constraint<TValue, TResult>(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            return new DelegateConstraintBuilder<TValue, TResult>(constraint, valueConverter);
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>  
        /// <param name="implementation">Delegate that represents the implementation of the constraint.</param>
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class</returns>
        public static DelegateConstraintBuilder<TValue, TResult> Constraint<TValue, TResult>(DelegateConstraint<TValue, TResult>.Implementation implementation = null)       
        {
            return new DelegateConstraintBuilder<TValue, TResult>(implementation);
        }

        internal static Exception NegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.New_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }        
    }
}
