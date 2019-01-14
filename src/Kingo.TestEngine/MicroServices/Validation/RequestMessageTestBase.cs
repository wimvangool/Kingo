using System;

namespace Kingo.MicroServices.Validation
{
    /// <summary>
    /// Serves as a base-class for all validation tests for a specific request message.
    /// </summary>
    public abstract class RequestMessageTestBase
    {
        private const int _DefaultMemberErrorCount = 0;

        #region [====== ValidationResult ======]

        private sealed class ValidationResult : IValidationResult
        {
            private readonly RequestMessageTestBase _test;
            private readonly ErrorInfo _errorInfo;

            public ValidationResult(RequestMessageTestBase test, ErrorInfo errorInfo)
            {
                _test = test;
                _errorInfo = errorInfo;
            }            

            public void AssertNoErrors()
            {
                if (_errorInfo.HasErrors)
                {
                    throw NewMessageNotValidException();
                }
            }

            public IValidationResult AssertMemberErrorCountIs(int expectedErrorCount)
            {
                if (_errorInfo.MemberErrors.Count == expectedErrorCount)
                {
                    return this;
                }
                throw NewUnexpectedErrorCountException(expectedErrorCount, _errorInfo.MemberErrors.Count);
            }            

            public IValidationResult AssertInstanceError(string expectedErrorMessage = null, StringComparison comparison = StringComparison.Ordinal)
            {                
                return AssertInstanceError(actualErrorMessage =>
                {
                    if (AreEqual(expectedErrorMessage, actualErrorMessage, comparison))
                    {
                        return;
                    }
                    throw NewUnexpectedInstanceErrorException(expectedErrorMessage, _errorInfo.Error, comparison);
                });                
            }            

            public IValidationResult AssertInstanceError(Action<string> assertCallback)
            {
                if (assertCallback == null)
                {
                    throw new ArgumentNullException(nameof(assertCallback));
                }
                if (string.IsNullOrEmpty(_errorInfo.Error))
                {
                    throw NewNoInstanceErrorException();
                }
                assertCallback.Invoke(_errorInfo.Error);
                return this;
            }

            public IValidationResult AssertMemberError(string memberName, string expectedErrorMessage = null, StringComparison comparison = StringComparison.Ordinal)
            {
                return AssertMemberError(memberName, actualErrorMessage =>
                {
                    if (AreEqual(expectedErrorMessage, actualErrorMessage, comparison))
                    {
                        return;
                    }
                    throw NewUnexpectedMemberErrorException(memberName, expectedErrorMessage, actualErrorMessage, comparison);
                });
            }           

            public IValidationResult AssertMemberError(string memberName, Action<string> assertCallback)
            {
                if (memberName == null)
                {
                    throw new ArgumentNullException(nameof(memberName));
                }
                if (assertCallback == null)
                {
                    throw new ArgumentNullException(nameof(assertCallback));
                }                
                if (_errorInfo.MemberErrors.TryGetValue(memberName, out var errorMessage))
                {
                    assertCallback.Invoke(errorMessage);
                    return this;
                }
                throw NewNoMemberErrorException(memberName);
            }            

            private static bool AreEqual(string expectedErrorMessage, string actualErrorMessage, StringComparison comparison) =>
                expectedErrorMessage == null || string.Compare(expectedErrorMessage, actualErrorMessage, comparison) == 0;

            private Exception NewMessageNotValidException()
            {
                var messageFormat = ExceptionMessages.RequestMessageTestBase_MessageNotValid;
                var message = string.Format(messageFormat, _errorInfo);
                return new AssertFailedException(message);
            }

            private static Exception NewUnexpectedErrorCountException(int expectedErrorCount, int actualErrorCount)
            {
                var messageFormat = ExceptionMessages.RequestMessageTestBase_UnexpectedErrorCount;
                var message = string.Format(messageFormat, expectedErrorCount, actualErrorCount);
                return new AssertFailedException(message);
            }

            private static Exception NewNoInstanceErrorException() =>
                new AssertFailedException(ExceptionMessages.RequestMessageTestBase_NoInstanceError);

            private static Exception NewNoMemberErrorException(string memberName)
            {
                var messageFormat = ExceptionMessages.RequestMessageTestBase_NoMemberError;
                var message = string.Format(messageFormat, memberName);
                return new AssertFailedException(message);
            }

            private static Exception NewUnexpectedInstanceErrorException(string expectedErrorMessage, string actualErrorMessage, StringComparison comparison)
            {
                var messageFormat = ExceptionMessages.RequestMessageTestBase_UnexpectedInstanceError;
                var message = string.Format(messageFormat, expectedErrorMessage, actualErrorMessage, comparison);
                return new AssertFailedException(message);
            }

            private static Exception NewUnexpectedMemberErrorException(string memberName, string expectedErrorMessage, string actualErrorMessage, StringComparison comparison)
            {
                var messageFormat = ExceptionMessages.RequestMessageTestBase_UnexpectedMemberError;
                var message = string.Format(messageFormat, memberName, expectedErrorMessage, actualErrorMessage, comparison);
                return new AssertFailedException(message);
            }            
        }

        #endregion

        /// <summary>
        /// Validates the specified <paramref name="message"/> and asserts that the message is valid.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>        
        protected virtual void AssertIsValid(IRequestMessage message) =>
            Validate(message, false).AssertNoErrors();

        /// <summary>
        /// Validates the specified <paramref name="message"/> and asserts that the result contains a certain amount of errors.
        /// </summary>
        /// <param name="message">The message to validate.</param>        
        /// <param name="haltOnFirstError">
        /// Indicates whether or not the validation function should halt on the first error it detects. Default is <c>false</c>.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>          
        protected IValidationResult AssertIsNotValid(IRequestMessage message, bool haltOnFirstError) =>
            AssertIsNotValid(message, _DefaultMemberErrorCount, haltOnFirstError);

        /// <summary>
        /// Validates the specified <paramref name="message"/> and asserts that the result contains a certain amount of errors.
        /// </summary>
        /// <param name="message">The message to validate.</param>
        /// <param name="expectedMemberErrorCount">
        /// The expected amount of errors. Default is 0.
        /// </param>
        /// <param name="haltOnFirstError">
        /// Indicates whether or not the validation function should halt on the first error it detects. Default is <c>false</c>.
        /// </param>
        /// <returns>The result of the validation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>  
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="expectedMemberErrorCount" /> is a negative number.
        /// </exception>
        protected virtual IValidationResult AssertIsNotValid(IRequestMessage message, int expectedMemberErrorCount = _DefaultMemberErrorCount, bool haltOnFirstError = false)
        {
            if (expectedMemberErrorCount < 0)
            {
                throw NewInvalidErrorCountException(expectedMemberErrorCount);
            }
            return Validate(message, haltOnFirstError).AssertMemberErrorCountIs(expectedMemberErrorCount);
        }               

        private ValidationResult Validate(IRequestMessage message, bool haltOnFirstError)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return new ValidationResult(this, message.Validate(haltOnFirstError));
        }

        private static Exception NewInvalidErrorCountException(int expectedErrorCount)
        {
            var messageFormat = ExceptionMessages.RequestMessageTestBase_InvalidErrorCount;
            var message = string.Format(messageFormat, expectedErrorCount);
            return new ArgumentOutOfRangeException(nameof(expectedErrorCount), message);
        }
    }
}
