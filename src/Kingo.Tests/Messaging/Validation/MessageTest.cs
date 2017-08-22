using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed partial class MessageTest
    {        
        #region [====== MessageWithValidator ======]

        [DataContract]
        private sealed class MessageWithValidator : RequestMessageBase
        {
            [DataMember]
            internal readonly int IntValue;

            [DataMember]
            internal readonly List<int> IntValues;

            public MessageWithValidator(int intValue, IEnumerable<int> values)
            {
                IntValue = intValue;                
                IntValues = new List<int>(values);
            }

            protected override IRequestMessageValidator CreateMessageValidator()
            {
                return base.CreateMessageValidator().Append<MessageWithValidator>((message, haltOnFirstError) =>
                {
                    const string errorMessage = "Error.";

                    var errorInfoBuilder = new ErrorInfoBuilder();

                    if (message.IntValue <= 0)
                    {
                        errorInfoBuilder.Add(errorMessage, nameof(message.IntValue));

                        if (haltOnFirstError)
                        {
                            return errorInfoBuilder.BuildErrorInfo();
                        }
                    }
                    if (message.IntValues == null || message.IntValues.Count == 0)
                    {
                        errorInfoBuilder.Add(errorMessage, nameof(message.IntValues));
                    }
                    return errorInfoBuilder.BuildErrorInfo();
                });
            }            
        }

        #endregion

        #region [====== MessageWithoutValidator ======]

        private sealed class MessageWithoutValidator { }

        #endregion                

        #region [====== Validate ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {
            var message = new MessageWithValidator(1, Enumerable.Range(0, 10));
            var errorInfo = message.Validate();

            Assert.IsNotNull(errorInfo);
            Assert.IsFalse(errorInfo.HasErrors);
        }

        [TestMethod]
        public void Validate_ReturnsExpectedErrors_IfMessageIsNotValid()
        {
            var message = new MessageWithValidator(0, Enumerable.Empty<int>());
            var errorInfo = message.Validate();

            Assert.IsNotNull(errorInfo);
            Assert.IsTrue(errorInfo.HasErrors);            
            Assert.AreEqual(2, errorInfo.MemberErrors.Count);
            Assert.AreEqual("Error.", errorInfo.MemberErrors["IntValue"]);
            Assert.AreEqual("Error.", errorInfo.MemberErrors["IntValues"]);
        }

        #endregion

        #region [====== TryGetValidator & Register ======]

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryGetValidator_Throws_IfMessageTypeIsNull()
        {
            IRequestMessageValidator validator;

            RequestMessageBase.TryGetMessageValidator(null, out validator);
        }

        [TestMethod]
        public void TryGetValidator_ReturnsFalse_IfNoValidatorHasBeenRegisteredForSpecifiedMessageType()
        {
            IRequestMessageValidator validator;

            Assert.IsFalse(RequestMessageBase.TryGetMessageValidator(typeof(MessageWithoutValidator), out validator));
            Assert.IsNull(validator);
        }

        [TestMethod]
        public void TryGetValidator_ReturnsTrue_IfValidatorHasBeenRegisteredForTheSpecifiedMessageType()
        {
            Assert.IsTrue(new MessageWithValidator(0, Enumerable.Empty<int>()).Validate().HasErrors);
            IRequestMessageValidator validator;

            Assert.IsTrue(RequestMessageBase.TryGetMessageValidator(typeof(MessageWithValidator), out validator));
            Assert.IsNotNull(validator);
            Assert.AreEqual("DelegateValidator<MessageWithValidator>", validator.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterValidator_Throws_IfValidatorIsNull()
        {
            RequestMessageBase.Register(null as IRequestMessageValidator<MessageWithValidator>);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterValidator_Throws_IfValidatorForSpecifiedMessageTypeHasAlreadyBeenRegistered()
        {
            Assert.IsTrue(new MessageWithValidator(0, Enumerable.Empty<int>()).Validate().HasErrors);

            try
            {
                RequestMessageBase.Register<MessageWithValidator>((message, haltOnFirstError) => ErrorInfo.Empty);
            }
            catch (ArgumentException exception)
            {
                Assert.IsTrue(exception.Message.StartsWith("Another validator for message of type 'MessageWithValidator' has already been registered."));
                throw;
            }
        }

        #endregion
    }
}
