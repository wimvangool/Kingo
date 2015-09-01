using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// Represents a builder that can be used to create instances of the <see cref="DelegateConstraint{T, S}" /> class.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public class DelegateConstraintBuilder<TValue, TResult>
    {
        private DelegateConstraint<TValue, TResult>.Implementation _implementation;
        private string _displayFormat;
        private string _errorFormat;
        private Func<object> _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <param name="implementation">Delegate that represents the implementation of the constraint.</param>
        public DelegateConstraintBuilder(DelegateConstraint<TValue, TResult>.Implementation implementation = null)
        {
            _implementation = implementation;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraintBuilder{T, S}" /> class.
        /// </summary>
        /// <param name="constraint">The implementation of the constraint.</param>
        /// <param name="valueConverter">Delegate used to convert the value of type <typeparamref name="TValue"/> to a value of type <typeparamref name="TResult"/>.</param>       
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="valueConverter"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraintBuilder(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            _implementation = CreateImplementation(constraint, valueConverter);
        }

        /// <summary>
        /// Sets the implementation of the constraint.
        /// </summary>
        /// <param name="implementation">Delegate that represents the implementation of the constraint.</param>
        public DelegateConstraintBuilder<TValue, TResult> WithImplementation(DelegateConstraint<TValue, TResult>.Implementation implementation)
        {
            _implementation = implementation;
            return this;
        }

        /// <summary>
        /// Sets the implementation of the constraint.
        /// </summary>
        /// <param name="constraint">The implementation of the constraint.</param> 
        /// <param name="valueConverter">Delegate used to convert the value of type <typeparamref name="TValue"/> to a value of type <typeparamref name="TResult"/>.</param>       
        /// <returns>This instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraint"/> or <paramref name="valueConverter"/> is <c>null</c>.
        /// </exception>
        public DelegateConstraintBuilder<TValue, TResult> WithImplementation(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            _implementation = CreateImplementation(constraint, valueConverter);
            return this;
        }   
        
        private static DelegateConstraint<TValue, TResult>.Implementation CreateImplementation(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            if (valueConverter == null)
            {
                throw new ArgumentNullException("valueConverter");
            }
            return delegate(TValue value, out TResult result)
            {
                if (constraint.Invoke(value))
                {
                    result = valueConverter.Invoke(value);
                    return true;
                }
                result = default(TResult);
                return false;
            };
        }

        /// <summary>
        /// Sets the format-string to use when creating a string-representation of the constraint.
        /// </summary>
        /// <param name="displayFormat">Format-string to use when creating a string-representation of the constraint.</param>
        /// <returns>This instance.</returns>
        public DelegateConstraintBuilder<TValue, TResult> WithDisplayFormat(string displayFormat)
        {
            _displayFormat = displayFormat;
            return this;
        }

        /// <summary>
        /// Sets the format-string to use when creating an error-message for the constraint.
        /// </summary>
        /// <param name="errorFormat">Format-string to use when creating an error-message for the constraint.</param>
        /// <returns>This instance.</returns>
        public DelegateConstraintBuilder<TValue, TResult> WithErrorFormat(string errorFormat)
        {
            _errorFormat = errorFormat;
            return this;
        }

        /// <summary>
        /// Sets the instance that contains all arguments for the display- and error format strings.
        /// </summary>
        /// <param name="arguments">The instance that contains all arguments for the display- and error format strings.</param>
        /// <returns>This instance.</returns>
        public DelegateConstraintBuilder<TValue, TResult> WithArguments(object arguments)
        {
            return WithArguments(() => arguments);
        }

        /// <summary>
        /// Sets the delegate that creates the instance that contains all arguments for the display- and error format strings.
        /// </summary>
        /// <param name="arguments">The delegate that creates the instance that contains all arguments for the display- and error format strings.</param>
        /// <returns>This instance.</returns>
        public DelegateConstraintBuilder<TValue, TResult> WithArguments(Func<object> arguments)
        {
            _arguments = arguments;
            return this;
        }

        /// <summary>
        /// Creates and returns a new <see cref="DelegateConstraint{T, S}" /> instance with specified information.
        /// </summary>        
        /// <exception cref="InvalidOperationException">
        /// No constraint implementation or string-representation has been set.
        /// </exception>
        /// <returns>A new <see cref="DelegateConstraint{T, S}" /> instance.</returns>
        public DelegateConstraint<TValue, TResult> BuildConstraint()
        {
            if (_implementation == null)
            {
                throw new InvalidOperationException(ExceptionMessages.DelegateConstraintBuilder_MissingConstraint);
            }
            if (_displayFormat == null)
            {
                throw new InvalidOperationException(ExceptionMessages.DelegateConstraintBuilder_MissingDisplayFormat);
            }
            return new DelegateConstraint<TValue, TResult>(_implementation, _displayFormat, _errorFormat, _arguments);
        }
    }
}
