using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Kingo.BuildingBlocks.Constraints;

namespace Kingo.BuildingBlocks
{
    /// <summary>
    /// A builder that can be used to build an instance of the <see cref="ErrorInfo" /> class.
    /// </summary>
    public class ErrorInfoBuilder : IErrorMessageReader
    {
        private readonly IFormatProvider _formatProvider;
        private readonly Lazy<IDictionary<string, IList<string>>> _memberErrors;
        private readonly Lazy<IList<string>> _errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfoBuilder" /> class.
        /// </summary>
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ErrorInfoBuilder(IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            _memberErrors = new Lazy<IDictionary<string, IList<string>>>(CreateMemberErrorDictionary);
            _errors = new Lazy<IList<string>>(CreateErrorList);
        }

        /// <summary>
        /// Returns the <see cref="IFormatProvider" /> that is used by this builder to format error messages.
        /// </summary>
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
        }        
        
        private IDictionary<string, IList<string>> MemberErrors
        {
            get { return _memberErrors.Value; }
        }

        private IList<string> Errors
        {
            get { return _errors.Value; }
        }

        public override string ToString()
        {
            return string.Format("{0} error(s)", _errors.Value.Count + _memberErrors.Value.Values.Sum(errors => errors.Count));
        }

        #region [====== Add ======]
      
        /// <inheritdoc />
        public void Add(IErrorMessage errorMessage, string memberName)
        {
            Add(Format(errorMessage), memberName);
        }

        /// <inheritdoc />
        public virtual void Add(string errorMessage, string memberName)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                return;
            }
            GetOrAddErrorList(memberName).Add(errorMessage);            
        }

        private IList<string> GetOrAddErrorList(string memberName)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                return Errors;                
            }
            IList<string> errorList;

            if (!MemberErrors.TryGetValue(memberName, out errorList))
            {
                MemberErrors.Add(memberName, errorList = CreateErrorList());
            }
            return errorList;
        }
        
        /// <summary>
        /// Formats the specified <paramref name="errorMessage"/> using the <see cref="FormatProvider" />.
        /// </summary>
        /// <param name="errorMessage">The message to format.</param>
        /// <returns>The formatted message.</returns>
        protected virtual string Format(IErrorMessage errorMessage)
        {
            return errorMessage == null ? null : errorMessage.ToString(FormatProvider);
        }

        #endregion

        /// <summary>
        /// Creates and returns a new <see cref="ErrorInfo"/> instance containing all added error messages.
        /// </summary>
        /// <returns>A new <see cref="ErrorInfo"/> instance.</returns>
        public ErrorInfo BuildErrorInfo()
        {
            var memberErrors = _memberErrors.IsValueCreated
                ? MemberErrors.Select(errors => new KeyValuePair<string, string>(errors.Key, ConvertToSingleErrorMessage(errors.Value)))
                : Enumerable.Empty<KeyValuePair<string, string>>();

            var error = _errors.IsValueCreated ? ConvertToSingleErrorMessage(Errors) : null;

            return BuildDataErrorInfo(memberErrors, error);
        }

        /// <summary>
        /// Converts the specified list of <paramref name="errorMessages"/> to a single error message.
        /// The list is guaranteed not to be empty.
        /// </summary>
        /// <param name="errorMessages">A list of error messages.</param>
        /// <returns>A single error message.</returns>
        protected virtual string ConvertToSingleErrorMessage(IList<string> errorMessages)
        {
            return errorMessages[0];
        }

        /// <summary>
        /// Creates and returns a new <see cref="ErrorInfo"/> instance containing all added error messages.
        /// </summary>
        /// <returns>A new <see cref="ErrorInfo"/> instance.</returns>
        protected virtual ErrorInfo BuildDataErrorInfo(IEnumerable<KeyValuePair<string, string>> memberErrors, string error)
        {
            return new ErrorInfo(memberErrors, error);
        }

        /// <summary>
        /// Creates and returns a new <see cref="IDictionary{T, S}" /> that will be used to store all error messages.
        /// </summary>
        /// <returns>A new <see cref="IDictionary{T, S}" />.</returns>
        protected virtual IDictionary<string, IList<string>> CreateMemberErrorDictionary()
        {
            return new Dictionary<string, IList<string>>();
        }

        /// <summary>
        /// Creates and returns a new <see cref="IList{T}" /> that will be used to store error messages.
        /// </summary>
        /// <returns>A new <see cref="IList{T}" /> instance.</returns>
        protected virtual IList<string> CreateErrorList()
        {
            return new List<string>();
        }
    }
}
