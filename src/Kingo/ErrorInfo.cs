using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Kingo
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
            return string.Format("{0} error(s).", ErrorCount);
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
        
        /// <summary>
        /// Merges two <see cref="ErrorInfo" /> instances into one uisng the specified <paramref name="builder"/>.
        /// </summary>
        /// <param name="left">The first error info instance.</param>
        /// <param name="right">The second error info instance.</param>
        /// <param name="builder">Builder that is used to build the merged instance.</param>
        /// <returns>
        /// A new <see cref="ErrorInfo" /> instance that contains all errors of both <paramref name="left"/> and <paramref name="right"/>.        
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="left"/> or <paramref name="right"/> is <c>null</c>.
        /// </exception>
        public static ErrorInfo Merge(ErrorInfo left, ErrorInfo right, ErrorInfoBuilder builder = null)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }
            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }
            if (builder == null)
            {
                builder = new ErrorInfoBuilder();
            }
            if (left.HasErrors && right.HasErrors)
            {
                // First, all error messages of the left instance are copied to the builder.
                var inheritanceLevel = ErrorInheritanceLevel.NotInherited;

                foreach (var memberError in left.MemberErrors)
                {
                    builder.Add(memberError.Value, memberError.Key, inheritanceLevel);
                }
                builder.Add(left.Error, null, inheritanceLevel);

                // Second, the inheritance level is increased, so that by default the left
                // error messages take precedence over the right error messages. Then, all
                // error message of the right instance are copied.
                inheritanceLevel = inheritanceLevel.Increment();

                foreach (var memberError in right.MemberErrors)
                {
                    builder.Add(memberError.Value, memberError.Key, inheritanceLevel);
                }
                builder.Add(right.Error, null, inheritanceLevel);

                return builder.BuildErrorInfo();
            }
            if (left.HasErrors)
            {
                return left;
            }
            if (right.HasErrors)
            {
                return right;
            }
            return Empty;
        }
    }
}
