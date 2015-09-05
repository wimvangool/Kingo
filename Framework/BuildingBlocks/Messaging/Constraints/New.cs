using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// This class contains factory methods to create objects purely for syntactical reasons.
    /// </summary>
    public static class New
    {
        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>  
        /// <param name="implementation">Delegate that represents the implementation of the constraint.</param>
        /// <param name="displayFormat">Format-string to use when creating a string-representation of the constraint.</param>
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class</returns>
        public static DelegateConstraintBuilder<TValue, TResult> Constraint<TValue, TResult>(DelegateConstraint<TValue, TResult>.Implementation implementation = null, string displayFormat = null)
        {
            return new DelegateConstraintBuilder<TValue, TResult>(implementation, displayFormat);
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>        
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static DelegateConstraintBuilder<TValue, TValue> Constraint<TValue>(Expression<Func<TValue, bool>> constraint)
        {
            return Constraint(constraint, value => value);
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
        public static DelegateConstraintBuilder<TValue, TResult> Constraint<TValue, TResult>(Expression<Func<TValue, bool>> constraint, Func<TValue, TResult> valueConverter)
        {
            return new DelegateConstraintBuilder<TValue, TResult>(constraint, valueConverter);
        }   

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>
        /// <param name="displayFormat">Format-string to use when creating a string-representation of the constraint.</param>
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>
        public static DelegateConstraintBuilder<TValue, TValue> Constraint<TValue>(Func<TValue, bool> constraint, string displayFormat)
        {
            return Constraint(constraint, value => value, displayFormat);
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <typeparam name="TValue">Type of the constraint value.</typeparam>
        /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>        
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>
        /// <param name="displayFormat">Format-string to use when creating a string-representation of the constraint.</param>
        /// <param name="valueConverter">Delegate used to convert the value of type <typeparamref name="TValue"/> to a value of type <typeparamref name="TResult"/>.</param>       
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="valueConverter"/> is <c>null</c>.
        /// </exception>        
        public static DelegateConstraintBuilder<TValue, TResult> Constraint<TValue, TResult>(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter, string displayFormat)
        {
            return new DelegateConstraintBuilder<TValue, TResult>(constraint, valueConverter, displayFormat);
        }        

        internal static Exception NegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.New_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }        
    }
}
