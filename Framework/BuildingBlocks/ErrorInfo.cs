using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific instance.
    /// </summary>
    [Serializable]    
    public sealed class ErrorInfo : IDataErrorInfo
    {                
        private readonly ReadOnlyDictionary<string, string> _memberErrors;        
        private readonly string _error;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo" /> class.
        /// </summary>        
        /// <param name="memberErrors">Error messages indexed by property- or fieldname.</param> 
        /// <param name="error">Error message for the whole object.</param>    
        /// <exception cref="ArgumentException">
        /// <paramref name="memberErrors"/> contains a duplicate key.
        /// </exception>          
        public ErrorInfo(IEnumerable<KeyValuePair<string, string>> memberErrors, string error = null)
        {
            _memberErrors = CreateDictionary(memberErrors);
            _error = error ?? string.Empty;
        }    
 
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo" /> class.
        /// </summary>        
        /// <param name="memberErrors">Error messages indexed by property- or fieldname.</param> 
        /// <param name="error">Error message for the whole object.</param>              
        public ErrorInfo(IDictionary<string, string> memberErrors, string error = null)
        {            
            _memberErrors = new ReadOnlyDictionary<string, string>(memberErrors);
            _error = error ?? string.Empty;         
        }

        private ErrorInfo(ReadOnlyDictionary<string, string> errors)
        {            
            _memberErrors = errors;
            _error = string.Empty;
        }

        /// <summary>
        /// Indicates whether or not this instance carries any error messages.
        /// </summary>
        public bool HasErrors
        {
            get { return ErrorCount > 0; }
        }

        /// <summary>
        /// Returns the number of error messages this instance carries.
        /// </summary>
        public int ErrorCount
        {
            get { return (_error.Length == 0 ? 0 : 1) + _memberErrors.Count; }
        }       

        /// <summary>
        /// Returns the error message that applies to the entire instance.
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Returns a collection of error messages per member.
        /// </summary>
        public IReadOnlyDictionary<string, string> MemberErrors
        {
            get { return _memberErrors; }
        }

        #region [====== IDataErrorInfo ======]

        string IDataErrorInfo.Error
        {
            get { return _error; }
        } 
        
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                string errorMessage;

                if (_memberErrors.TryGetValue(columnName, out errorMessage))
                {
                    return errorMessage;
                }
                return null;
            }
        }                              

        #endregion                                     

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} error(s).", _memberErrors.Count);
        }

        #region [====== Factory Methods ======]
        
        /// <summary>
        /// An instance of the <see cref="ErrorInfo" /> class without any errors.
        /// </summary>       
        public static readonly ErrorInfo Empty = new ErrorInfo(EmptyDictionary());                    

        private static ReadOnlyDictionary<string, string> EmptyDictionary()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());    
        }       
 
        private static ReadOnlyDictionary<string, string> CreateDictionary(IEnumerable<KeyValuePair<string, string>> memberErrors)
        {
            if (memberErrors == null)
            {
                return EmptyDictionary();
            }
            var dictionary = new Dictionary<string, string>();

            foreach (var memberError in memberErrors)
            {
                dictionary.Add(memberError.Key, memberError.Value);
            }
            return new ReadOnlyDictionary<string, string>(dictionary);
        }

        #endregion
    }
}
