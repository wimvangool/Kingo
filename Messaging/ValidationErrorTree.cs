using System.Collections.Generic;
using System.ComponentModel.Resources;
using System.Diagnostics;
using System.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific message.
    /// </summary>
    [Serializable]    
    public sealed class ValidationErrorTree : IDataErrorInfo
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _message;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyDictionary<string, string> _errors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<string> _error;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ValidationErrorTree[] _childErrorTrees;        

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTree" /> class.
        /// </summary>
        /// <param name="message">The message these errors were found on.</param>
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public ValidationErrorTree(object message, IDictionary<string, string> errors)
            : this(message, errors, null) { }
 
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTree" /> class.
        /// </summary>
        /// <param name="message">The message these errors were found on.</param>
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>
        /// <param name="childErrors">Trees containing errors for any child-messages of the specified <paramref name="message"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public ValidationErrorTree(object message, IDictionary<string, string> errors, IEnumerable<ValidationErrorTree> childErrors)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            _message = message;
            _errors = new ReadOnlyDictionary<string, string>(errors);
            _error = new Lazy<string>(CreateError);
            _childErrorTrees = Trim(childErrors);
        }

        private ValidationErrorTree(object message, ReadOnlyDictionary<string, string> errors, IEnumerable<ValidationErrorTree> childErrors)
        {
            _message = message;
            _errors = errors;
            _childErrorTrees = Trim(childErrors);
        }

        #region [====== IDataErrorInfo ======]

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string errorMessage;

                if (_errors.TryGetValue(columnName, out errorMessage))
                {
                    return errorMessage;
                }
                return null;
            }
        }

        string IDataErrorInfo.Error
        {
            get { return _error.Value; }
        }

        private string CreateError()
        {
            return _errors.Count == 0 ? null : Concatenate(_errors.Values);
        }

        internal static string Concatenate(IEnumerable<string> errorMessages)
        {
            return string.Join(Environment.NewLine, errorMessages.Where(error => !string.IsNullOrWhiteSpace(error)));
        }

        #endregion

        /// <summary>
        /// Returns the message these errors relate to.
        /// </summary>
        public object Message
        {
            get { return _message; }
        }        

        /// <summary>
        /// Returns the number of errors that were detected on the related message.
        /// </summary>
        public int TotalErrorCount
        {
            get { return _errors.Count + ChildErrors.Sum(errorTree => errorTree.TotalErrorCount); }
        }

        /// <summary>
        /// Returns the errors that were detected on the message. The key represent a property or field,
        /// the value contains the error message.
        /// </summary>
        public IDictionary<string, string> Errors
        {
            get { return _errors; }
        }

        /// <summary>
        /// Returns a collection of <see cref="ValidationErrorTree">error trees</see> that contain the errors of child-messages, if present.
        /// </summary>        
        [DebuggerDisplay("Count = {_childErrorTrees.Length}")]
        public IEnumerable<ValidationErrorTree> ChildErrors
        {
            get { return _childErrorTrees; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} contains {1} error(s) and {2} child error(s).",
                _message.GetType().Name,
                _errors.Count,
                _childErrorTrees.Sum(errorTree => errorTree.Errors.Count));
        }

        #region [====== Factory Methods ======]
        
        /// <summary>
        /// Creates and returns a new instance of the <see cref="ValidationErrorTree" /> class without any errors.
        /// </summary>
        /// <param name="message">Type of a message.</param>
        /// <returns>A new instance of the <see cref="ValidationErrorTree" /> class without any errors.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static ValidationErrorTree NoErrors(object message)
        {
            return new ValidationErrorTree(message, new ReadOnlyDictionary<string, string>(), null);
        }                        
        
        internal static ValidationErrorTree Merge(object message, IEnumerable<ValidationErrorTree> childErrors)
        {
            return new ValidationErrorTree(message, new ReadOnlyDictionary<string, string>(), childErrors);
        }

        /// <summary>
        /// Merges the a parent tree with a set of child trees.
        /// </summary>
        /// <param name="errorTree">The parent tree.</param>
        /// <param name="childErrors">The child trees.</param>
        /// <returns>A merged <see cref="ValidationErrorTree" />.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="errorTree"/> is <c>null</c>.
        /// </exception>
        public static ValidationErrorTree Merge(ValidationErrorTree errorTree, IEnumerable<ValidationErrorTree> childErrors)
        {            
            if (errorTree == null)
            {
                throw new ArgumentNullException("errorTree");
            }
            return new ValidationErrorTree(errorTree._message, errorTree._errors, childErrors);
        }

        internal bool TryCreateInvalidMessageException(out InvalidMessageException exception)
        {
            if (TotalErrorCount == 0)
            {
                exception = null;
                return false;
            }
            exception = new InvalidMessageException(_message, ExceptionMessages.InvalidMessageException_InvalidMessage, this);
            return true;
        }

        private static ValidationErrorTree[] Trim(IEnumerable<ValidationErrorTree> childErrors)
        {
            if (childErrors == null)
            {
                return new ValidationErrorTree[0];
            }
            var trimmedList = from errorTree in childErrors
                              where errorTree != null && errorTree.TotalErrorCount > 0
                              select errorTree;

            return trimmedList.ToArray();
        }

        #endregion
    }
}
