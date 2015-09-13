using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Messaging.Constraints
{
    /// <summary>
    /// Represents a constraint implementation that is composed of delegates.
    /// </summary>
    /// <typeparam name="TMessage">Type of a certain message.</typeparam>
    /// <typeparam name="TValue">Type of the value to check.</typeparam>
    /// <typeparam name="TResult">Type of the result the value is converted to.</typeparam>
    public sealed class DelegateConstraint<TMessage, TValue, TResult> : ConstraintWithErrorMessage<TMessage, TValue, TResult>, IConstraintWithErrorMessage<TMessage>
    {
        /// <summary>
        /// Represents a delegate that contains the implementation of a <see cref="DelegateConstraint{TMessage, T, S}" />.
        /// </summary>        
        public delegate bool Implementation(TValue value, TMessage message, out TResult result);

        private readonly Implementation _implementation;        
        private readonly StringTemplate _errorMessage;
        private readonly Func<TMessage, object> _errorMessageArgumentFactory;              
              
        internal DelegateConstraint(Implementation implementation, StringTemplate errorMessage, Func<TMessage, object> errorMessageArgumentFactory)
        {            
            _implementation = implementation;            
            _errorMessage = errorMessage;
            _errorMessageArgumentFactory = errorMessageArgumentFactory;
        }

        /// <inheritdoc />
        public StringTemplate ErrorMessage
        {
            get { return _errorMessage; }
        }

        /// <inheritdoc />
        public object ErrorMessageArguments(TMessage message)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            return _errorMessageArgumentFactory == null ? null : _errorMessageArgumentFactory.Invoke(message);
        }

        /// <inheritdoc />
        public override bool IsSatisfiedBy(TValue value, TMessage message, out TResult result, out IConstraintWithErrorMessage<TMessage> failedConstraint)
        {
            if (ReferenceEquals(message, null))
            {
                throw new ArgumentNullException("message");
            }
            if (_implementation.Invoke(value, message, out result))
            {
                failedConstraint = null;
                return true;
            }
            failedConstraint = this;
            return false;
        }        
    }
}
