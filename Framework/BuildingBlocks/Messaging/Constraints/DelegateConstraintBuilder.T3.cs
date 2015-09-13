using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a builder that can be used to create instances of the <see cref="DelegateConstraint{TMessage, T, S}" /> class.
    /// </summary>
    /// <typeparam name="TMessage">Type of a certain message.</typeparam>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public sealed class DelegateConstraintBuilder<TMessage, TValue, TResult>
    {        
        private readonly DelegateConstraint<TMessage, TValue, TResult>.Implementation _implementation;        
        private StringTemplate _errorMessage;
        private Func<TMessage, object> _errorMessageArgumentFactory;

        internal DelegateConstraintBuilder(Expression<Func<TValue, TMessage, bool>> constraint, Func<TValue, TResult> valueConverter)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _implementation = CreateImplementation(constraint.Compile(), valueConverter);            
        }

        internal DelegateConstraintBuilder(Func<TValue, TMessage, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            _implementation = CreateImplementation(constraint, valueConverter);            
        }               
        
        internal DelegateConstraintBuilder(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            _implementation = CreateImplementation(constraint, valueConverter);            
        }

        internal DelegateConstraintBuilder(DelegateConstraint<TMessage, TValue, TResult>.Implementation implementation)
        {
            if (implementation == null)
            {
                throw new ArgumentNullException("implementation");
            }
            _implementation = implementation;            
        }      

        /// <summary>
        /// Sets the format-string to use when creating an error-message for the constraint.
        /// </summary>
        /// <param name="errorMessage">Format-string to use when creating an error-message for the constraint.</param>
        /// <returns>This instance.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public DelegateConstraintBuilder<TMessage, TValue, TResult> WithErrorMessage(string errorMessage)
        {
            return WithErrorMessage(StringTemplate.Parse(errorMessage));
        }

        /// <summary>
        /// Sets the format-string to use when creating an error-message for the constraint.
        /// </summary>
        /// <param name="errorMessage">Format-string to use when creating an error-message for the constraint.</param>
        /// <returns>This instance.</returns>        
        public DelegateConstraintBuilder<TMessage, TValue, TResult> WithErrorMessage(StringTemplate errorMessage)
        {
            _errorMessage = errorMessage;
            return this;
        }

        /// <summary>
        /// Sets the instance that contains all arguments for the display- and error format strings.
        /// </summary>
        /// <param name="errorMessageArguments">The instance that contains all arguments for the error message template.</param>
        /// <returns>This instance.</returns>
        public DelegateConstraintBuilder<TMessage, TValue, TResult> WithErrorMessageArguments(object errorMessageArguments)
        {            
            _errorMessageArgumentFactory = errorMessageArguments == null ? null as Func<TMessage, object> : (message => errorMessageArguments);
            return this;
        }

        /// <summary>
        /// Sets the instance that contains all arguments for the display- and error format strings.
        /// </summary>
        /// <param name="errorMessageArgumentFactory">Delegate that returns the instance that contains all arguments for the error message template.</param>
        /// <returns>This instance.</returns>
        public DelegateConstraintBuilder<TMessage, TValue, TResult> WithErrorMessageArguments(Func<TMessage, object> errorMessageArgumentFactory)
        {
            _errorMessageArgumentFactory = errorMessageArgumentFactory;
            return this;
        }   

        /// <summary>
        /// Creates and returns a new <see cref="DelegateConstraint{TMessage, T, S}" /> instance with specified information.
        /// </summary>                
        /// <returns>A new <see cref="DelegateConstraint{TMessage, T, S}" /> instance.</returns>
        public IConstraintWithErrorMessage<TMessage, TValue, TResult> BuildConstraint()
        {            
            return new DelegateConstraint<TMessage, TValue, TResult>(_implementation, _errorMessage, _errorMessageArgumentFactory);
        }

        private static DelegateConstraint<TMessage, TValue, TResult>.Implementation CreateImplementation(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            if (valueConverter == null)
            {
                throw new ArgumentNullException("valueConverter");
            }
            return delegate(TValue value, TMessage message, out TResult result)
            {
                if (ReferenceEquals(message, null))
                {
                    throw new ArgumentNullException("message");
                }
                if (constraint.Invoke(value))
                {
                    result = valueConverter.Invoke(value);
                    return true;
                }
                result = default(TResult);
                return false;
            };
        }  

        private static DelegateConstraint<TMessage, TValue, TResult>.Implementation CreateImplementation(Func<TValue, TMessage, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            if (valueConverter == null)
            {
                throw new ArgumentNullException("valueConverter");
            }
            return delegate(TValue value, TMessage message, out TResult result)
            {                
                if (ReferenceEquals(message, null))
                {
                    throw new ArgumentNullException("message");
                }
                if (constraint.Invoke(value, message))
                {
                    result = valueConverter.Invoke(value);
                    return true;
                }
                result = default(TResult);
                return false;
            };
        }        
    }
}
