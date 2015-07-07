using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Syztem.ComponentModel
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific message.
    /// </summary>
    [Serializable]    
    public sealed class ValidationErrorTree : IDataErrorInfo
    {        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyDictionary<string, string> _errors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<string> _error;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ValidationErrorTree[] _childErrorTrees;        

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTree" /> class.
        /// </summary>        
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>                
        public ValidationErrorTree(IDictionary<string, string> errors)
            : this(errors, null) { }
 
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorTree" /> class.
        /// </summary>        
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>
        /// <param name="childErrors">Trees containing errors for any child-messages.</param>        
        public ValidationErrorTree(IDictionary<string, string> errors, IEnumerable<ValidationErrorTree> childErrors)
        {            
            _errors = new ReadOnlyDictionary<string, string>(errors);
            _error = new Lazy<string>(CreateError);
            _childErrorTrees = Trim(childErrors);
        }

        private ValidationErrorTree(ReadOnlyDictionary<string, string> errors, IEnumerable<ValidationErrorTree> childErrors)
        {            
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
            return string.Format("{0} error(s) and {1} child error(s).",                
                _errors.Count,
                _childErrorTrees.Sum(errorTree => errorTree.Errors.Count));
        }

        #region [====== Factory Methods ======]
        
        /// <summary>
        /// An instance of the <see cref="ValidationErrorTree" /> class without any errors.
        /// </summary>       
        public static readonly ValidationErrorTree NoErrors = new ValidationErrorTree(EmptyDictionary(), null);        
        
        internal static ValidationErrorTree Merge(IEnumerable<ValidationErrorTree> childErrors)
        {
            return new ValidationErrorTree(EmptyDictionary(), childErrors);
        }

        private static ReadOnlyDictionary<string, string> EmptyDictionary()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());    
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
            return new ValidationErrorTree(errorTree._errors, childErrors);
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
