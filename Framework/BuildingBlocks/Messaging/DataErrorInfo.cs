using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Kingo.BuildingBlocks.Messaging
{
    /// <summary>
    /// Represents a tree of errors that have been detected on a specific message.
    /// </summary>
    [Serializable]    
    public sealed class DataErrorInfo : IDataErrorInfo
    {        
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ReadOnlyDictionary<string, string> _errors;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Lazy<string> _error;                      
 
        /// <summary>
        /// Initializes a new instance of the <see cref="DataErrorInfo" /> class.
        /// </summary>        
        /// <param name="errors">Error-messages indexed by property- or fieldname.</param>               
        public DataErrorInfo(IDictionary<string, string> errors)
        {            
            _errors = new ReadOnlyDictionary<string, string>(errors);
            _error = new Lazy<string>(CreateError);            
        }

        private DataErrorInfo(ReadOnlyDictionary<string, string> errors)
        {            
            _errors = errors;            
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
        /// Returns the errors that were detected on the message. The key represent a property or field,
        /// the value contains the error message.
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
        public static readonly DataErrorInfo NoErrors = new DataErrorInfo(EmptyDictionary());
        
        internal static readonly DataErrorInfo[] EmptyList = new DataErrorInfo[0];      

        private static ReadOnlyDictionary<string, string> EmptyDictionary()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());    
        }        

        #endregion
    }
}
