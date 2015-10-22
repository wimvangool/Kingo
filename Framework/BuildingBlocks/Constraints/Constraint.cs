using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IConstraintWithErrorMessage" /> interface.
    /// </summary>
    public abstract class Constraint : IConstraintWithErrorMessage
    {        
        private readonly Identifier _name;
        private readonly StringTemplate _errorMessage;        

        /// <summary>
        /// Initializes a new instance of the <see cref="Constraint{T}" /> class.
        /// </summary>
        /// <param name="name">Name of this constraint.</param>
        /// <param name="errorMessage">Error message of this constraint.</param>        
        internal Constraint(StringTemplate errorMessage, Identifier name)
        {            
            _name = name ?? DefaultName;
            _errorMessage = errorMessage ?? DefaultErrorMessage;
        }

        #region [====== Name & ErrorMessage ======]

        /// <inheritdoc />
        public Identifier Name
        {
            get { return _name; }
        }

        /// <inheritdoc />
        public StringTemplate ErrorMessage
        {
            get { return _errorMessage; }
        }        

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithName(string name)
        {
            return WithNameCore(Identifier.ParseOrNull(name));
        }

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithName(Identifier name)
        {
            return WithNameCore(name);
        }

        internal abstract IConstraintWithErrorMessage WithNameCore(Identifier name);

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithErrorMessage(string errorMessage)
        {
            return WithErrorMessageCore(StringTemplate.ParseOrNull(errorMessage));
        }

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithErrorMessage(StringTemplate errorMessage)
        {
            return WithErrorMessageCore(errorMessage);
        }

        internal abstract IConstraintWithErrorMessage WithErrorMessageCore(StringTemplate errorMessage);

        /// <summary>
        /// Represents the default name of a constraint when no name has been specified explicitly.
        /// </summary>
        public static readonly Identifier DefaultName = Identifier.Parse("constraint");

        /// <summary>
        /// Represents the default error message of a constraint when no error message has been specified explicitly.
        /// </summary>
        public static readonly StringTemplate DefaultErrorMessage = StringTemplate.Parse(ErrorMessages.BasicConstraints_Default);        

        /// <summary>
        /// Returns the left template if <paramref name="left"/> is not <c>null</c>; otherwise returns <paramref name="right"/>.
        /// </summary>
        /// <param name="left">Left template.</param>
        /// <param name="right">Right template.</param>
        /// <returns>
        /// <paramref name="left"/> if it is not <c>null</c>; otherwise <paramref name="right"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="right"/> is not in a correct format.
        /// </exception>
        public static StringTemplate SelectBetween(StringTemplate left, string right)
        {
            return SelectBetween(left, StringTemplate.ParseOrNull(right));
        }
 
        private static StringTemplate SelectBetween(StringTemplate left, StringTemplate right)
        {
            return left ?? right;
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
                throw new ArgumentNullException("constraints");
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
                throw new ArgumentNullException("constraints");
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
