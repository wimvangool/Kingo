using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YellowFlare.MessageProcessing.Requests
{
    /// <summary>
    /// Represents the result of a validation-step of a message or other component.
    /// </summary>
    public sealed class MessageErrorInfo : IDataErrorInfo
    {        
        private readonly List<ValidationResult> _validationResults;
        private readonly Dictionary<string, string> _errorMessagesPerMember;        

        private MessageErrorInfo(List<ValidationResult> validationResults, Dictionary<string, string> errorMessagesPerMember)
        {            
            _validationResults = validationResults;
            _errorMessagesPerMember = errorMessagesPerMember;
        }

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
            get
            {
                if (_validationResults == null)
                {
                    return string.Empty;
                }
                return Concatenate(_validationResults.Select(result => result.ErrorMessage));
            }
        }

        /// <summary>
        /// This value is used to mark a <see cref="Message" /> as invalid when no validation of it has yet taken place.
        /// </summary>
        internal static readonly MessageErrorInfo NotYetValidated = new MessageErrorInfo(null, null);                      

        /// <summary>
        /// Performs validation of the instance that is specified by the <paramref name="validationContext"/>
        /// and returns any validation errors in the form of a <see cref="MessageErrorInfo" /> instance.
        /// </summary>
        /// <param name="validationContext">The validation context to use.</param>
        /// <returns>
        /// A new <see cref="MessageErrorInfo" />-instance containing all validation-errors, or <c>null</c>
        /// if validation completed succesfully.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validationContext"/> is <c>null</c>.
        /// </exception>
        public static MessageErrorInfo CreateErrorInfo(ValidationContext validationContext)
        {
            if (validationContext == null)
            {
                throw new ArgumentNullException("validationContext");
            }
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(validationContext.ObjectInstance, validationContext, validationResults, true);

            if (isValid)
            {
                return null;
            }
            var errorMessagesPerMember = CreateErrorMessagesPerMember(validationResults);

            return new MessageErrorInfo(validationResults, errorMessagesPerMember);
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
            return string.Join(Environment.NewLine, errorMessages);
        }         
    }
}
