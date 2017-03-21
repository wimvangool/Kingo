using System;

namespace Kingo.Messaging.Validation.Constraints
{
    /// <summary>
    /// When implemented by a class, represents a constraint with a name and associated error message.
    /// </summary>
    public interface IConstraintWithErrorMessage : IConstraint
    {
        /// <summary>
        /// Name of the constraint.
        /// </summary>
        Identifier Name
        {
            get;
        }

        /// <summary>
        /// Error message associated with this constraint.
        /// </summary>
        StringTemplate ErrorMessage
        {
            get;
        }

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">New name of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is not valid <see cref="Identifier" />.
        /// </exception>
        IConstraintWithErrorMessage WithName(string name);

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">New name of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="name"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is <c>null</c>.
        /// </exception>        
        IConstraintWithErrorMessage WithName(Identifier name);

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">New error message of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="errorMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="errorMessage"/> is not in a correct format.
        /// </exception>
        IConstraintWithErrorMessage WithErrorMessage(string errorMessage);

        /// <summary>
        /// Creates and returns a copy of this constraint, assigning the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage">New error message of the constraint.</param>
        /// <returns>A copy of this constraint with the specified <paramref name="errorMessage"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>        
        IConstraintWithErrorMessage WithErrorMessage(StringTemplate errorMessage);
    }
}
