using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific message.
    /// </summary>
    [Serializable]    
    public sealed class DataErrorInfo : IDataErrorInfo
    {                
        private readonly ReadOnlyDictionary<string, string> _errors;        
        private readonly string _error;                      
 
        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfo" /> class.
        /// </summary>        
        /// <param name="errors">Error messages indexed by property- or fieldname.</param> 
        /// <param name="error">Error message for the whole object.</param>              
        public DataErrorInfo(IDictionary<string, string> errors, string error = null)
        {            
            _errors = new ReadOnlyDictionary<string, string>(errors);
            _error = error ?? string.Empty;         
        }

        private DataErrorInfo(ReadOnlyDictionary<string, string> errors)
        {            
            _errors = errors;
            _error = string.Empty;
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
            get { return _error; }
        }                

        #endregion                       

        /// <summary>
        /// Returns the errors that were detected on the message. The key represent a property or field,
        /// the value contains the error message. This collection is read-only.
        /// </summary>
        public IDictionary<string, string> Errors
        {
            get { return _errors; }
        }        

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("{0} error(s).", _errors.Count);
        }

        #region [====== Factory Methods ======]
        
        /// <summary>
        /// An instance of the <see cref="DataErrorInfo" /> class without any errors.
        /// </summary>       
        public static readonly DataErrorInfo Empty = new DataErrorInfo(EmptyDictionary());                    

        private static ReadOnlyDictionary<string, string> EmptyDictionary()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());    
        }        

        #endregion
    }
}
