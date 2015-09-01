using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.ComponentModel.Constraints
{
    /// <summary>
    /// Represents a constraint implementation that is composed of delegates.
    /// </summary>
    /// <typeparam name="TValue">Type of the value to check.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public class DelegateConstraint<TValue, TResult> : ConstraintWithErrorMessage<TValue, TResult>, IConstraintWithErrorMessage
    {
        /// <summary>
        /// Represents a delegate that contains the implementation of a <see cref="DelegateConstraint{T, S}" />.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public delegate bool Implementation(TValue value, out TResult result);

        private readonly Implementation _implementation;
        private readonly StringTemplate _displayFormat;
        private readonly StringTemplate _errorMessage;
        private readonly Func<object> _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{T, S}" /> class.
        /// </summary>
        /// <param name="implementation">Delegate that represents the implementation of this constraint.</param>   
        /// <param name="displayFormat">Format string that will be used to create a string-representation of this constraint.</param>
        /// <param name="errorMessage">The error message that belongs to this constraint.</param>
        /// <param name="arguments">
        /// If specified, this delegate will be used to create the argument-instance to replace all placeholder values of the constraint
        /// within the <paramref name="displayFormat"/> and <paramref name="errorMessage"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="implementation"/> or <paramref name="displayFormat"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="displayFormat"/> or <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        public DelegateConstraint(Implementation implementation, string displayFormat, string errorMessage = null, Func<object> arguments = null)            
        {
            if (implementation == null)
            {
                throw new ArgumentNullException("implementation");
            }
            if (displayFormat == null)
            {
                throw new ArgumentNullException("displayFormat");
            }
            _implementation = implementation;  
            _displayFormat = StringTemplate.Parse(displayFormat);
            _errorMessage = StringTemplate.Parse(errorMessage ?? ConstraintErrors.MemberConstraints_Default);
            _arguments = arguments;
        }       

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value, out TResult result, out IConstraintWithErrorMessage failedConstraint)
        {
            if (_implementation.Invoke(value, out result))
            {
                failedConstraint = null;
                return true;
            }
            failedConstraint = this;
            return false;
        }

        /// <inheritdoc />
        public StringTemplate FormatErrorMessage(IFormatProvider formatProvider = null)
        {
            return _errorMessage.Format(MemberConstraints.ConstraintId, ConstraintArguments(), formatProvider);
        }

        /// <inheritdoc />
        public override string ToString(string memberName)
        {
            return _displayFormat
                .Format(MemberConstraints.MemberId, new { Name = memberName })
                .Format(MemberConstraints.ConstraintId, ConstraintArguments())
                .ToString();
        }   
     
        private object ConstraintArguments()
        {
            return _arguments == null ? null : _arguments.Invoke();
        }
    }
}
