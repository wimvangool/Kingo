using System;
using Kingo.Resources;

namespace Kingo.Messaging.Validation
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IConstraintWithErrorMessage" /> interface.
    /// </summary>
    public abstract class Constraint : IConstraintWithErrorMessage
    {        
        private readonly Lazy<Identifier> _name;
        private readonly Lazy<StringTemplate> _errorMessage;                                                  

        internal Constraint(Constraint constraint = null)
        {
            if (constraint == null)
            {
                _name = new Lazy<Identifier>(() => NameIfNotSpecified);
                _errorMessage = new Lazy<StringTemplate>(() => ErrorMessageIfNotSpecified);
            }
            else
            {
                _name = constraint._name;
                _errorMessage = constraint._errorMessage;
            }
        }

        internal Constraint(Constraint constraint, StringTemplate errorMessage)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            _name = constraint._name;
            _errorMessage = new Lazy<StringTemplate>(() => errorMessage ?? ErrorMessageIfNotSpecified);
        }

        internal Constraint(Constraint constraint, Identifier name)
        {
            if (constraint == null)
            {
                throw new ArgumentNullException(nameof(constraint));
            }
            _name = new Lazy<Identifier>(() => name ?? NameIfNotSpecified);
            _errorMessage = constraint._errorMessage;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public Identifier Name => _name.Value;

        /// <summary>
        /// Returns the name of this constraint if it was not explicitly specified.
        /// </summary>
        protected virtual Identifier NameIfNotSpecified => DefaultName;

        /// <inheritdoc />
        public StringTemplate ErrorMessage => _errorMessage.Value;

        /// <summary>
        /// Returns the error message of this constraint if it was not explicitly specified.
        /// </summary>
        protected virtual StringTemplate ErrorMessageIfNotSpecified => DefaultErrorMessage;

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithName(string name) => WithNameCore(Identifier.ParseOrNull(name));

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithName(Identifier name) => WithNameCore(name);

        internal abstract IConstraintWithErrorMessage WithNameCore(Identifier name);

        internal Exception NewWithNameNotSupportedException()
        {
            var messageFormat = ExceptionMessages.Constraint_WithNameNotSupported;
            var message = string.Format(messageFormat, GetType().Name);
            return new NotSupportedException(message);
        }             

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithErrorMessage(string errorMessage) => WithErrorMessageCore(StringTemplate.ParseOrNull(errorMessage));

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithErrorMessage(StringTemplate errorMessage) => WithErrorMessageCore(errorMessage);

        internal abstract IConstraintWithErrorMessage WithErrorMessageCore(StringTemplate errorMessage);

        internal Exception NewWithErrorMessageNotSupportedException()
        {
            var messageFormat = ExceptionMessages.Constraint_WithErrorMessageNotSupported;
            var message = string.Format(messageFormat, GetType().Name);
            return new NotSupportedException(message);
        }

        /// <summary>
        /// Represents the default name of a constraint when no name has been specified explicitly.
        /// </summary>
        public static readonly Identifier DefaultName = Identifier.Parse("constraint");

        /// <summary>
        /// Represents the default error message of a constraint when no error message has been specified explicitly.
        /// </summary>
        public static readonly StringTemplate DefaultErrorMessage = StringTemplate.Parse(ErrorMessages.BasicConstraints_Default);           

        #endregion

        #region [====== Visitor ======]

        /// <inheritdoc />
        public virtual void AcceptVisitor(IConstraintVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }
            visitor.Visit(this);
        }

        #endregion

        #region [====== Any & All ======]

        /// <summary>
        /// Returns a logical OR-constraint composed of the specified <paramref name="constraints"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="constraints">A set of constraints.</param>
        /// <returns>A logical OR-constraint composed of the specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        public static IConstraintWithErrorMessage<TValue> Any<TValue>(params IConstraint<TValue>[] constraints)
        {
            if (constraints == null)
            {
                throw new ArgumentNullException(nameof(constraints));
            }
            if (constraints.Length == 0)
            {
                return new NullConstraint<TValue>();
            }
            if (constraints.Length == 1)
            {
                return new ConstraintWrapper<TValue>(constraints[0]);
            }
            var constraint = constraints[0].Or(constraints[1]);

            for (int index = 2; index < constraints.Length; index++)
            {
                constraint = constraint.Or(constraints[index]);
            }
            return constraint;
        }

        /// <summary>
        /// Returns a logical AND-constraint composed of the specified <paramref name="constraints"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value to check.</typeparam>
        /// <param name="constraints">A set of constraints.</param>
        /// <returns>A logical AND-constraint composed of the specified <paramref name="constraints"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="constraints"/> is <c>null</c>.
        /// </exception>
        public static IConstraint<TValue> All<TValue>(params IConstraint<TValue>[] constraints)
        {
            if (constraints == null)
            {
                throw new ArgumentNullException(nameof(constraints));
            }
            if (constraints.Length == 0)
            {
                return new NullConstraint<TValue>();
            }
            if (constraints.Length == 1)
            {
                return constraints[0];
            }
            var constraint = constraints[0].And(constraints[1]);

            for (int index = 2; index < constraints.Length; index++)
            {
                constraint = constraint.And(constraints[index]);
            }
            return constraint;
        }

        #endregion
    }
}
