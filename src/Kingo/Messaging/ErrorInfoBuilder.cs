using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Kingo.Messaging
{
    /// <summary>
    /// A builder that can be used to build an instance of the <see cref="ErrorInfo" /> class.
    /// </summary>
    public class ErrorInfoBuilder : ErrorMessageReader
    {
        private readonly IFormatProvider _formatProvider;
        private readonly Dictionary<string, SortedErrorMessageList> _memberErrors;
        private readonly SortedErrorMessageList _instanceErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfoBuilder" /> class.
        /// </summary>
        /// <param name="formatProvider">Optional <see cref="IFormatProvider" /> to use when formatting error messages.</param>
        public ErrorInfoBuilder(IFormatProvider formatProvider = null)
        {
            _formatProvider = formatProvider ?? CultureInfo.CurrentCulture;
            _memberErrors = new Dictionary<string, SortedErrorMessageList>();
            _instanceErrors = new SortedErrorMessageList();
        }

        /// <inheritdoc />
        protected override IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            int errorCount = 0;
            int errorCountOfNonInheritedErrors = 0;

            foreach (var errorList in ErrorMessages())
            {
                errorCount += errorList.Count;
                errorCountOfNonInheritedErrors += errorList.CountNonInheritedErrors();
            }
            return string.Format(SortedErrorMessageList.DebugStringFormat, errorCount, errorCountOfNonInheritedErrors);
        }

        private IEnumerable<SortedErrorMessageList> ErrorMessages()
        {
            return _memberErrors.Select(errorsPerMember => errorsPerMember.Value).Concat(new[] { _instanceErrors });
        }

        #region [====== Add ======]

        /// <inheritdoc />
        public override void Add(string errorMessage, string memberName, ErrorInheritanceLevel inheritanceLevel)
        {
            if (errorMessage == null)
            {
                throw new ArgumentNullException(nameof(errorMessage));
            }
            GetOrAddErrorList(memberName).Add(inheritanceLevel, errorMessage);
        }

        private SortedErrorMessageList GetOrAddErrorList(string memberName)
        {
            if (string.IsNullOrEmpty(memberName))
            {
                return _instanceErrors;             
            }
            SortedErrorMessageList errorList;

            if (!_memberErrors.TryGetValue(memberName, out errorList))
            {
                _memberErrors.Add(memberName, errorList = new SortedErrorMessageList());
            }
            return errorList;
        }               

        #endregion

        /// <summary>
        /// Creates and returns a new <see cref="ErrorInfo"/> instance containing all added error messages.
        /// </summary>
        /// <returns>A new <see cref="ErrorInfo"/> instance.</returns>
        public ErrorInfo BuildErrorInfo()
        {
            var memberErrors = _memberErrors.Select(Collapse);               
            var instanceError = ConvertToSingleErrorMessage(_instanceErrors);

            return BuildDataErrorInfo(memberErrors, instanceError);
        }

        private KeyValuePair<string, string> Collapse(KeyValuePair<string, SortedErrorMessageList> errorMessagesPerMember)
        {
            var memberName = errorMessagesPerMember.Key;
            var errorMessage = ConvertToSingleErrorMessage(errorMessagesPerMember.Value);

            return new KeyValuePair<string, string>(memberName, errorMessage);
        }

        /// <summary>
        /// Converts the specified list of <paramref name="errorMessages"/> to a single error message. By default,
        /// the first error message with the lowest inheritance level is selected.       
        /// </summary>
        /// <param name="errorMessages">A list of error messages.</param>
        /// <returns>A single error message.</returns>
        protected virtual string ConvertToSingleErrorMessage(SortedErrorMessageList errorMessages)
        {
            return errorMessages.Count == 0 ? null : errorMessages.First().Value;
        }

        /// <summary>
        /// Creates and returns a new <see cref="ErrorInfo"/> instance containing all added error messages.
        /// </summary>
        /// <returns>A new <see cref="ErrorInfo"/> instance.</returns>
        protected virtual ErrorInfo BuildDataErrorInfo(IEnumerable<KeyValuePair<string, string>> memberErrors, string instanceError)
        {
            return new ErrorInfo(memberErrors, instanceError);
        }        
    }
}
