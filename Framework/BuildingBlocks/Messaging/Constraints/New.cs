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
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class.
        /// </summary>          
        /// <param name="implementation">Delegate that represents the implementation of the constraint.</param>        
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class</returns>
        public static DelegateConstraintBuilder<TMessage, TValue, TResult> Constraint<TMessage, TValue, TResult>(DelegateConstraint<TMessage, TValue, TResult>.Implementation implementation = null)
        {
            return new DelegateConstraintBuilder<TMessage, TValue, TResult>(implementation);
        }        

        #region [====== Func<TValue, TMessage, bool> ======]

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class.
        /// </summary>        
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>        
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>        
        public static DelegateConstraintBuilder<TMessage, TValue, TValue> Constraint<TMessage, TValue>(Func<TValue, TMessage, bool> constraint)
        {
            return Constraint(constraint, value => value);
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class.
        /// </summary>               
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>                
        /// <param name="valueConverter">Delegate used to convert the value of type <typeparamref name="TValue"/> to a value of type <typeparamref name="TResult"/>.</param>       
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="valueConverter"/> is <c>null</c>.
        /// </exception>                
        public static DelegateConstraintBuilder<TMessage, TValue, TResult> Constraint<TMessage, TValue, TResult>(Func<TValue, TMessage, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            return new DelegateConstraintBuilder<TMessage, TValue, TResult>(constraint, valueConverter);
        }

        #endregion        

        #region [====== Func<TValue, bool> ======]

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class.
        /// </summary>        
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>        
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> is <c>null</c>.
        /// </exception>        
        public static DelegateConstraintBuilder<TMessage, TValue, TValue> Constraint<TMessage, TValue>(Func<TValue, bool> constraint)
        {
            return Constraint<TMessage, TValue, TValue>(constraint, value => value);
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class.
        /// </summary>               
        /// <param name="constraint">Delegate that contains the implementation of the constraint.</param>        
        /// <param name="valueConverter">Delegate used to convert the value of type <typeparamref name="TValue"/> to a value of type <typeparamref name="TResult"/>.</param>       
        /// <returns>A new instance of the <see cref="DelegateConstraintBuilder{TMessage, T, S}" /> class</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="valueConverter"/> is <c>null</c>.
        /// </exception>               
        public static DelegateConstraintBuilder<TMessage, TValue, TResult> Constraint<TMessage, TValue, TResult>(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            return new DelegateConstraintBuilder<TMessage, TValue, TResult>(constraint, valueConverter);
        }

        #endregion

        internal static Exception NegativeIndexException(int index)
        {
            var messageFormat = ExceptionMessages.New_NegativeIndex;
            var message = string.Format(messageFormat, index);
            return new ArgumentOutOfRangeException("index", message);
        }        
    }
}
