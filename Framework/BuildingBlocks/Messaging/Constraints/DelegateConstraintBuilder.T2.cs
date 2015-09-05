using System;
using System.Linq.Expressions;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a builder that can be used to create instances of the <see cref="DelegateConstraint{T, S}" /> class.
    /// </summary>
    /// <typeparam name="TValue">Type of the constraint value.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public sealed class DelegateConstraintBuilder<TValue, TResult>
    {        
        private readonly DelegateConstraint<TValue, TResult>.Implementation _implementation;
        private readonly StringTemplate _displayFormat;
        private StringTemplate _errorFormat;
        private Func<object> _arguments;
        
        internal DelegateConstraintBuilder(DelegateConstraint<TValue, TResult>.Implementation implementation, string displayFormat)
        {
            if (implementation == null)
            {
                throw new ArgumentNullException("implementation");
            }           
            _implementation = implementation;
            _displayFormat = StringTemplate.Parse(displayFormat);
        }
        
        internal DelegateConstraintBuilder(Expression<Func<TValue, bool>> constraint, Func<TValue, TResult> valueConverter)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException("constraint");
            }
            _implementation = CreateImplementation(constraint.Compile(), valueConverter);
            _displayFormat = DisplayFormatFactory.InferDisplayFormat(constraint);
        }
        
        internal DelegateConstraintBuilder(Func<TValue, bool> constraint, Func<TValue, TResult> valueConverter, string displayFormat)
        {
            _implementation = CreateImplementation(constraint, valueConverter);
            _displayFormat = StringTemplate.Parse(displayFormat);
        }                       

        /// <summary>
        /// Sets the format-string to use when creating an error-message for the constraint.
        /// </summary>
        /// <param name="errorFormat">Format-string to use when creating an error-message for the constraint.</param>
        /// <returns>This instance.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorFormat"/> is not <c>null</c> and is not in a correct format.
        /// </exception>
        public DelegateConstraintBuilder<TValue, TResult> WithErrorFormat(string errorFormat)
        {
            _errorFormat = errorFormat == null ? null : StringTemplate.Parse(errorFormat);
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
        /// <returns>A new <see cref="DelegateConstraint{T, S}" /> instance.</returns>
        public IConstraintWithErrorMessage<TValue, TResult> BuildConstraint()
        {            
            return new DelegateConstraint<TValue, TResult>(_implementation, _displayFormat, _errorFormat, _arguments);
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
    }
}
