using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed class RequestMessageTestBaseTest : RequestMessageTestBase
    {
        #region [====== MessageToValidate ======]

        private sealed class MessageToValidate : RequestMessageBase
        {
            public MessageToValidate(bool addInstanceError = false) :
                this(Guid.NewGuid(), 1, addInstanceError) { }

            public MessageToValidate(Guid id, int version, bool addInstanceError = false)
            {
                Id = id;
                Version = version;
                AddInstanceError = addInstanceError;
            }

            public Guid Id
            {
                get;                
            }

            public int Version
            {
                get;                
            }

            private bool AddInstanceError
            {
                get;
            }

            protected override IRequestMessageValidator CreateMessageValidator()
            {
                return base.CreateMessageValidator().Append<MessageToValidate>((message, haltOnFirstError) =>
                {
                    const string errorMessage = "Error";
                    var errorInfoBuilder = new ErrorInfoBuilder();

                    if (message.AddInstanceError)
                    {
                        errorInfoBuilder.Add(errorMessage, null);
                    }
                    if (message.Id == Guid.Empty)
                    {
                        errorInfoBuilder.Add(errorMessage, nameof(Id));
                    }
                    if (message.Version <= 0)
                    {
                        errorInfoBuilder.Add(errorMessage, nameof(Version));
                    }
                    return errorInfoBuilder.BuildErrorInfo();
                });
            }
        }

        #endregion

        #region [====== AssertIsValid ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertIsValid_Throws_IfMessageIsNull()
        {
            AssertIsValid(null);
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertIsValid_Throws_IfMessageIsNotValid()
        {
            AssertIsValid(new MessageToValidate(true));
        }

        [TestMethod]
        public void AssertIsValid_Succeeds_IfMessageIsValid()
        {
            AssertIsValid(new MessageToValidate());
        }

        #endregion

        #region [====== AssertIsNotValid ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertIsNotValid_Throws_IfMessageIsNull()
        {
            AssertIsNotValid(null, 1);
        }        

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void AssertIsNotValid_Throws_IfExpectedErrorCountIsNegative()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(), -1);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("The specified number of expected errors (-1) is invalid. This number must be greater than or equal to 1."));
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertIsNotValid_Throws_IfMessageIsValid()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(), 1);
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The number of expected validation errors (1) does not match the actual amount of validation errors (0).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertIsNotValid_Throws_IfNumberOfExpectedValidationErrorsDoesNotMatchActualAmountOfValidationErrors()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(true), 1);
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The number of expected validation errors (1) does not match the actual amount of validation errors (0).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void AssertIsNotValid_ReturnsExpectedResult_IfNumberOfExpectedValidationErrorsMatchesActualAmountOfValidationErrors()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(true)));
        }

        #endregion

        #region [====== AssertInstanceError(string) ======]

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertInstanceError_Throws_IfResultDoesNotContainAnInstanceError()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(Guid.NewGuid(), 1)).AssertInstanceError();
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("An instance error was expected but the result of the validation contains no instance error.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertInstanceError_Throws_IfInstanceErrorDoesNotMatchExpectedInstanceError()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError("Bla");
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The expected instance error (Bla) does not match the actual instance error (Error) based on the type of comparison specified (Ordinal).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertInstanceError_Throws_IfInstanceErrorDoesNotMatchExpectedInstanceError_BasedOnTheSpecifiedComparison()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError("error");
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The expected instance error (error) does not match the actual instance error (Error) based on the type of comparison specified (Ordinal).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void AssertInstanceError_ReturnsExpectedResult_IfInstanceErrorMatchesExpectedError()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError("Error"));
        }

        [TestMethod]
        public void AssertInstanceError_ReturnsExpectedResult_IfInstanceErrorMatchesExpectedError_BasedOnTheSpecifiedComparison()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError("error", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region [====== AssertInstanceError(Action<string>) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertInstanceError_Throws_IfAssertCallbackIsNull()
        {
            AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError(null as Action<string>);
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertInstanceError_Throws_IfResultDoesNotContainAnInstanceError_And_AssertCallbackIsSpecified()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertInstanceError(errorMessage => { });
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("An instance error was expected but the result of the validation contains no instance error.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertInstanceError_Throws_IfAssertCallbackFails()
        {
            var exceptionToThrow = new MetaAssertFailedException(string.Empty, null);

            try
            {
                AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError(errorMessage =>
                {
                    throw exceptionToThrow;
                });
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        public void AssertInstanceError_ReturnsExpectedResult_IfAssertCallbackDoesNotFail()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(true)).AssertInstanceError(errorMessage => { }));
        }

        #endregion

        #region [====== AssertMemberError(string) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertMemberError_Throws_IfMemberNameIsNull()
        {
            AssertIsNotValid(new MessageToValidate(true)).AssertMemberError(null);
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertMemberError_Throws_IfMemberContainsNoError()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(true)).AssertMemberError("Id");
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("An error for member 'Id' was expected, but the result contains no error for this member.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertMemberError_Throws_IfMemberErrorDoesNotMatchExpectedError()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", "Bla");
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The expected error for member 'Id' (Bla) does not match the actual error (Error) based on the type of comparison specified (Ordinal).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertMemberError_Throws_IfMemberErrorDoesNotMatchExpectedError_BasedOnTheSpecifiedComparison()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", "error");
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("The expected error for member 'Id' (error) does not match the actual error (Error) based on the type of comparison specified (Ordinal).", exception.Message);
                throw;
            }
        }

        [TestMethod]
        public void AssertMemberError_ReturnsExpectedResult_IfIfMemberErrorMatchesExpectedError()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", "Error"));
        }

        [TestMethod]
        public void AssertMemberError_ReturnsExpectedResult_IfIfMemberErrorMatchesExpectedError_BasedOnSpecifiedComparison()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", "error", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region [====== AssertMemberError(Action<string>) ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AssertMemberError_Throws_IfAssertCallbackIsNull()
        {
            AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", null as Action<string>);
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertMemberError_Throws_IfMemberDoesNotContainError_And_AssertCallbackIsSpecified()
        {
            try
            {
                AssertIsNotValid(new MessageToValidate(true)).AssertMemberError("Id", errorMessage => { });
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreEqual("An error for member 'Id' was expected, but the result contains no error for this member.", exception.Message);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(MetaAssertFailedException))]
        public void AssertMemberError_Throws_IfAssertCallbackFails()
        {
            var exceptionToThrow = new MetaAssertFailedException(string.Empty, null);

            try
            {
                AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", errorMessage =>
                {
                    throw exceptionToThrow;
                });
            }
            catch (MetaAssertFailedException exception)
            {
                Assert.AreSame(exceptionToThrow, exception);
                throw;
            }
        }

        [TestMethod]
        public void AssertMemberError_ReturnsExpectedResult_IfAssertCallbackDoesNotFail()
        {
            Assert.IsNotNull(AssertIsNotValid(new MessageToValidate(Guid.Empty, 1), 1).AssertMemberError("Id", errorMessage => { }));
        }

        #endregion

        protected override Exception NewAssertFailedException(string message, Exception innerException = null) =>
            new MetaAssertFailedException(message, innerException);
    }
}
