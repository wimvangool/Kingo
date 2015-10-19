using System;
using Kingo.BuildingBlocks.Resources;

namespace Kingo.BuildingBlocks.Constraints
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IConstraintWithErrorMessage" /> interface.
    /// </summary>
    public abstract class ConstraintWithErrorMessage : IConstraintWithErrorMessage
    {        
        private readonly Identifier _name;
        private readonly StringTemplate _errorMessage;        

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstraintWithErrorMessage{T}" /> class.
        /// </summary>
        /// <param name="name">Name of this constraint.</param>
        /// <param name="errorMessage">Error message of this constraint.</param>        
        internal ConstraintWithErrorMessage(StringTemplate errorMessage, Identifier name)
        {            
            _name = name ?? DefaultName;
            _errorMessage = errorMessage ?? DefaultErrorMessage;
        }

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
            return WithNameCore(Identifier.Parse(name));
        }

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithName(Identifier name)
        {
            return WithNameCore(name);
        }

        internal abstract IConstraintWithErrorMessage WithNameCore(Identifier name);

        IConstraintWithErrorMessage IConstraintWithErrorMessage.WithErrorMessage(string errorMessage)
        {
            return WithErrorMessageCore(StringTemplate.Parse(errorMessage));
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
        public static readonly StringTemplate DefaultErrorMessage = StringTemplate.Parse(ErrorMessages.MemberConstraints_Default);        

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
            return SelectBetween(left, StringTemplate.Parse(right));
        }
 
        private static StringTemplate SelectBetween(StringTemplate left, StringTemplate right)
        {
            return left ?? right;
        }
    }
}
