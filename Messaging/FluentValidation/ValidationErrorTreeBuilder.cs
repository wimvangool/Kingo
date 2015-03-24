using System.Collections.Generic;

namespace System.ComponentModel.FluentValidation
{
    /// <summary>
    /// Represents a builder that can be used to build new instances of the <see cref="ValidationErrorTree" /> class.
    /// </summary>
    public class ValidationErrorTreeBuilder
    {                
        private readonly Dictionary<string, IList<string>> _errors;
        private readonly List<ValidationErrorTree> _childErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTreeBuilder" /> class.
        /// </summary>        
        public ValidationErrorTreeBuilder()
        {            
            _errors = new Dictionary<string, IList<string>>();
            _childErrors = new List<ValidationErrorTree>();
        }

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessage">The error-message for this member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessage)
        {
            Add(memberName, new ErrorMessage(memberName, errorMessage));
        }

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessageFormat">The error-message for this member.</param>
        /// <param name="arg0">The first argument for <paramref name="errorMessageFormat"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessageFormat, object arg0)
        {
            Add(memberName, new ErrorMessage(memberName, errorMessageFormat, arg0));
        }

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessageFormat">The error-message for this member.</param>
        /// <param name="arg0">The first argument for <paramref name="errorMessageFormat"/>.</param>
        /// <param name="arg1">The second argument for <paramref name="errorMessageFormat"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessageFormat, object arg0, object arg1)
        {
            Add(memberName, new ErrorMessage(memberName, errorMessageFormat, arg0, arg1));
        }

        /// <summary>
        /// Add the specified error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessageFormat">The error-message for this member.</param>
        /// <param name="arguments">The arguments for <paramref name="errorMessageFormat"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessageFormat"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, string errorMessageFormat, params object[] arguments)
        {
            Add(memberName, new ErrorMessage(memberName, errorMessageFormat, arguments));
        }

        /// <summary>
        /// Adds the specified <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessage">The error-message to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>        
        public void Add(string memberName, ErrorMessage errorMessage)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }
            IList<string> errors;

            if (!_errors.TryGetValue(memberName, out errors))
            {
                _errors.Add(memberName, errors = new List<string>());
            }
            errors.Add(errorMessage.ToString());
        }

        /// <summary>
        /// Adds a tree of child error-messages to this builder.
        /// </summary>
        /// <param name="errorTree">The errors to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorTree"/> is <c>null</c>.
        /// </exception>
        public void AddChildErrors(ValidationErrorTree errorTree)
        {
            if (errorTree == null)
            {
                throw new ArgumentNullException("errorTree");
            }
            _childErrors.Add(errorTree);
        }        

        /// <summary>
        /// Creates and returns a new instance of the <see cref="ValidationErrorTree" /> class
        /// containing all specified errors and child-errors.
        /// </summary>
        /// <returns>A new instance of the <see cref="ValidationErrorTree" /> class.</returns>
        public ValidationErrorTree BuildErrorTree()
        {
            return new ValidationErrorTree(_errors, _childErrors);
        }        
    }
}
