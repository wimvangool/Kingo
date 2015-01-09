using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a builder that can be used to build new instances of the <see cref="ValidationErrorTree" /> class.
    /// </summary>
    public sealed class ValidationErrorTreeBuilder
    {
        private readonly Type _messageType;
        private readonly Dictionary<string, string> _errors;
        private readonly List<ValidationErrorTree> _childErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTreeBuilder" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message that is being validated.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public ValidationErrorTreeBuilder(Type messageType)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }
            _messageType = messageType;
            _errors = new Dictionary<string, string>();
            _childErrors = new List<ValidationErrorTree>();
        }

        /// <summary>
        /// Adds an error-message for the specified <paramref name="memberName"/>.
        /// </summary>
        /// <param name="memberName">The validated member.</param>
        /// <param name="errorMessage">The error-message for this member.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="memberName"/> or <paramref name="errorMessage"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An error-message for <paramref name="memberName"/> has already been added to this builder.
        /// </exception>
        public void AddError(string memberName, string errorMessage)
        {
            if (memberName == null)
            {
                throw new ArgumentNullException("memberName");
            }
            if (errorMessage == null)
            {
                throw new ArgumentNullException("errorMessage");
            }            
            _errors.Add(memberName, errorMessage);
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
        /// Attempts to create and return a new instance of the <see cref="ValidationErrorTree" /> class
        /// containing all specified errors and child-errors.
        /// </summary>
        /// <param name="errorTree">
        /// If this method returns <c>true</c>, refers to a new instance of the <see cref="ValidationErrorTree" /> class;
        /// otherwise <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if any errors or child-errors were added to this builder; otherwise <c>false</c>.
        /// </returns>
        public bool TryBuildErrorTree(out ValidationErrorTree errorTree)
        {
            if (_errors.Count == 0 && _childErrors.Count == 0)
            {
                errorTree = null;
                return false;
            }
            errorTree = BuildErrorTree();
            return true;
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="ValidationErrorTree" /> class
        /// containing all specified errors and child-errors.
        /// </summary>
        /// <returns>A new instance of the <see cref="ValidationErrorTree" /> class.</returns>
        public ValidationErrorTree BuildErrorTree()
        {
            return new ValidationErrorTree(_messageType, _errors, _childErrors);
        }
    }
}
