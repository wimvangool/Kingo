using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kingo.Reflection;

namespace Kingo.MicroServices.DataContracts
{
    internal sealed class RequestMessageValidator : IRequestMessageValidator
    {
        #region [====== IsNotValidResult ======]

        private sealed class IsNotValidResult : IIsNotValidResult
        {            
            private readonly ValidationResult[] _validationErrors;

            public IsNotValidResult(IEnumerable<ValidationResult> validationErrors)
            {                
                _validationErrors = validationErrors.ToArray();
            }

            public void And(Action<IValidationErrorCollection> errorValidator)
            {
                if (errorValidator == null)
                {
                    throw new ArgumentNullException(nameof(errorValidator));
                }
                errorValidator.Invoke(new ValidationErrorCollection(_validationErrors));
            }
        }

        #endregion

        private readonly object _request;

        public RequestMessageValidator(object request)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public void IsValid()
        {
            if (_request.IsNotValid(out var validationErrors))
            {
                throw NewRequestNotValidException(_request, validationErrors);
            }
        }

        public IIsNotValidResult IsNotValid(int expectedNumberOfErrors)
        {
            if (expectedNumberOfErrors < 1)
            {
                throw NewInvalidNumberOfExpectedErrorsSpecifiedException(expectedNumberOfErrors);
            }
            if (_request.IsNotValid(out var validationErrors))
            {
                if (expectedNumberOfErrors == validationErrors.Count)
                {
                    return new IsNotValidResult(validationErrors);
                }
                throw NewUnexpectedNumberOfValidationErrorsOccurredException(expectedNumberOfErrors, validationErrors.Count);
            }
            throw NewRequestValidException(_request);
        }        

        private static Exception NewInvalidNumberOfExpectedErrorsSpecifiedException(int expectedNumberOfErrors)
        {
            var messageFormat = ExceptionMessages.Request_InvalidNumberOfExpectedErrorsSpecified;
            var message = string.Format(messageFormat, expectedNumberOfErrors);
            return new ArgumentOutOfRangeException(nameof(expectedNumberOfErrors), message);
        }

        private static Exception NewUnexpectedNumberOfValidationErrorsOccurredException(int expectedNumberOfErrors, int actualNumberOfErrors)
        {
            var messageFormat = ExceptionMessages.Request_UnexpectedNumberOfErrorsOccurred;
            var message = string.Format(messageFormat, expectedNumberOfErrors, actualNumberOfErrors);
            return new TestFailedException(message);
        }

        private static Exception NewRequestNotValidException(object instance, ICollection<ValidationResult> results)
        {
            var messageFormat = ExceptionMessages.Request_InstanceNotValid;
            var message = string.Format(messageFormat, instance.GetType().FriendlyName(), results.Count);
            return new TestFailedException(message);
        }

        private static Exception NewRequestValidException(object instance)
        {
            var messageFormat = ExceptionMessages.Request_InstanceValid;
            var message = string.Format(messageFormat, instance.GetType().FriendlyName());
            return new TestFailedException(message);
        }
    }
}
