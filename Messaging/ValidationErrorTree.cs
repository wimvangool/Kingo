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
        private readonly Type _messageType;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyDictionary<string, string> _errors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<string> _error;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ValidationErrorTree[] _childErrorTrees;        

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTree" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message these errors were found on.</param>
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>        
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public ValidationErrorTree(Type messageType, IDictionary<string, string> errors)
            : this(messageType, errors, null) { }
 
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTree" /> class.
        /// </summary>
        /// <param name="messageType">Type of the message these errors were found on.</param>
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>
        /// <param name="childErrorTrees">Trees containing errors for any child-messages of the specified <paramref name="messageType"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public ValidationErrorTree(Type messageType, IDictionary<string, string> errors, IEnumerable<ValidationErrorTree> childErrorTrees)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException("messageType");
            }
            _messageType = messageType;
            _errors = new ReadOnlyDictionary<string, string>(errors);
            _error = new Lazy<string>(CreateError);
            _childErrorTrees = childErrorTrees == null ? new ValidationErrorTree[0] : childErrorTrees.ToArray();
        }

        private ValidationErrorTree(Type messageType, ReadOnlyDictionary<string, string> errors, IEnumerable<ValidationErrorTree> childErrorTrees)
        {
            _messageType = messageType;
            _errors = errors;
            _childErrorTrees = childErrorTrees == null ? new ValidationErrorTree[0] : childErrorTrees.ToArray();
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
        /// Returns the type of the message these errors relate to.
        /// </summary>
        public Type MessageType
        {
            get { return _messageType; }
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
                _messageType.Name,
                _errors.Count,
                _childErrorTrees.Sum(errorTree => errorTree.Errors.Count));
        }

        #region [====== Factory Methods ======]
        
        /// <summary>
        /// Creates and returns a new instance of <see cref="ValidationErrorTree" /> class without any errors.
        /// </summary>
        /// <param name="messageType">Type of a message.</param>
        /// <returns>A new instance of the <see cref="ValidationErrorTree" /> class without any errors.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageType"/> is <c>null</c>.
        /// </exception>
        public static ValidationErrorTree NoErrors(Type messageType)
        {
            return new ValidationErrorTree(messageType, new ReadOnlyDictionary<string, string>(), null);
        }                        

        internal static ValidationErrorTree Merge(Type messageType, ICollection<ValidationErrorTree> childErrorTrees)
        {
            return new ValidationErrorTree(messageType, new ReadOnlyDictionary<string, string>(), childErrorTrees);
        }

        internal static ValidationErrorTree Merge(ValidationErrorTree errorTree, ICollection<ValidationErrorTree> childErrorTrees)
        {
            if (childErrorTrees.Count == 0)
            {
                return errorTree;
            }
            return new ValidationErrorTree(errorTree._messageType, errorTree._errors, childErrorTrees);
        }

        internal bool TryCreateInvalidMessageException(object message, out InvalidMessageException exception)
        {
            if (TotalErrorCount == 0)
            {
                exception = null;
                return false;
            }
            exception = new InvalidMessageException(message, ExceptionMessages.InvalidMessageException_InvalidMessage, this);
            return true;
        }

        #endregion
    }
}
