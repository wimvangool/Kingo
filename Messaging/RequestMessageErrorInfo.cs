using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// Represents the result of a validation-step of a message or other component.
    /// </summary>
    internal sealed class RequestMessageErrorInfo : IDataErrorInfo
    {                
        private readonly Dictionary<string, string> _errorMessagesPerMember;
        private readonly string _errorMessage;

        private RequestMessageErrorInfo(Dictionary<string, string> errorMessagesPerMember, string errorMessage)
        {                        
            _errorMessagesPerMember = errorMessagesPerMember;
            _errorMessage = errorMessage;
        }

        internal RequestMessageErrorInfo(RequestMessageErrorInfo errorInfo)
        {            
            _errorMessagesPerMember = errorInfo._errorMessagesPerMember == null ? null : new Dictionary<string, string>(errorInfo._errorMessagesPerMember);
            _errorMessage = errorInfo._errorMessage;
        }

        #region [====== IDataErrorInfo ======]

        /// <inheritdoc />
        public string this[string columnName]
        {
            get
            {
                string errorMessage;

                if (_errorMessagesPerMember != null && _errorMessagesPerMember.TryGetValue(columnName, out errorMessage))
                {
                    return errorMessage;
                }
                return null;
            }
        }

        /// <inheritdoc />
        public string Error
        {
            get { return _errorMessage;}
        }

        #endregion

        /// <summary>
        /// This value is used to mark a <see cref="RequestMessage{TMessage}" /> as invalid when no validation of it has yet taken place.
        /// </summary>
        internal static readonly RequestMessageErrorInfo NotYetValidated = new RequestMessageErrorInfo(null, null);                      

        /// <summary>
        /// Performs validation of the instance that is specified by the <paramref name="validationContext"/>
        /// and returns any validation errors in the form of a <see cref="RequestMessageErrorInfo" /> instance.
        /// </summary>
        /// <param name="validationContext">The validation context to use.</param>
        /// <returns>
        /// A new <see cref="RequestMessageErrorInfo" />-instance containing all validation-errors, or <c>null</c>
        /// if validation completed succesfully.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validationContext"/> is <c>null</c>.
        /// </exception>
        internal static RequestMessageErrorInfo CreateErrorInfo(ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }
            RequestMessageLabelProvider.Add(validationContext.ObjectInstance);

            try
            {
                var validationResults = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);

                if (isValid)
                {
                    return null;
                }                
                var errorMessagesPerMember = CreateErrorMessagesPerMember(validationResults);
                var errorMessage = CreateErrorMessage(validationResults);

                return new RequestMessageErrorInfo(errorMessagesPerMember, errorMessage);
            }
            finally
            {
                RequestMessageLabelProvider.Remove(validationContext.ObjectInstance);
            }            
        }

        private static string CreateErrorMessage(IEnumerable<ValidationResult> validationResults)
        {
            if (validationResults == null)
            {
                return string.Empty;
            }
            return Concatenate(validationResults.Select(result => result.ErrorMessage));
        }

        private static Dictionary<string, string> CreateErrorMessagesPerMember(IEnumerable<ValidationResult> validationResults)
        {
            var errorMessageBuilder = new Dictionary<string, List<string>>();

            foreach (var result in validationResults)
            {
                AppendErrorMessage(errorMessageBuilder, result);
            }
            var errorMessagesPerMember = new Dictionary<string, string>();

            foreach (var member in errorMessageBuilder)
            {
                errorMessagesPerMember.Add(member.Key, Concatenate(member.Value));
            }
            return errorMessagesPerMember;
        }

        private static void AppendErrorMessage(IDictionary<string, List<string>> errorMessageBuilder, ValidationResult result)
        {
            foreach (var memberName in result.MemberNames)
            {
                List<string> errorMessages;

                if (!errorMessageBuilder.TryGetValue(memberName, out errorMessages))
                {
                    errorMessageBuilder.Add(memberName, errorMessages = new List<string>());
                }
                errorMessages.Add(result.ErrorMessage);
            }
        }        
        
        internal static string Concatenate(IEnumerable<string> errorMessages)
        {            
            return string.Join(Environment.NewLine, errorMessages.Where(error => !string.IsNullOrWhiteSpace(error)));
        }

        #region [====== MessageErrorTree Factory ======]

        internal static MessageErrorTree CreateErrorTree<TMessage>(RequestMessage<TMessage> message, IEnumerable<IRequestMessage> attachedMessages)
            where TMessage : RequestMessage<TMessage>
        {
            var messageType = typeof(TMessage);
            var errors = message.ErrorInfo == null ? null : message.ErrorInfo._errorMessagesPerMember;
            var childErrors = CreateErrorTrees(attachedMessages);

            return new MessageErrorTree(messageType, errors, childErrors);
        }

        private static IEnumerable<MessageErrorTree> CreateErrorTrees(IEnumerable<IRequestMessage> messages)
        {
            if (messages == null)
            {
                return Enumerable.Empty<MessageErrorTree>();
            }
            var childErrors = new LinkedList<MessageErrorTree>();            

            foreach (var message in messages)
            {
                MessageErrorTree childErrorTree;

                if (message.IsNotValid(out childErrorTree))
                {
                    childErrors.AddLast(childErrorTree);
                }
            }
            return childErrors;
        }

        #endregion
    }
}
