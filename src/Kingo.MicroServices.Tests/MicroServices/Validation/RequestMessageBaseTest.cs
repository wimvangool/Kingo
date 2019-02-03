using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Validation
{
    [TestClass]
    public sealed class RequestMessageBaseTest
    {        
        #region [====== MessageWithValidator ======]

        [DataContract]
        private sealed class RequestMessageWithValidator : RequestMessageBase
        {
            [DataMember]
            internal readonly int IntValue;

            [DataMember]
            internal readonly List<int> IntValues;

            public RequestMessageWithValidator(int intValue, IEnumerable<int> values)
            {
                IntValue = intValue;                
                IntValues = new List<int>(values);
            }

            protected override IRequestMessageValidator CreateMessageValidator()
            {
                return base.CreateMessageValidator().Append<RequestMessageWithValidator>((message, haltOnFirstError) =>
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

        #region [====== Validate ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {
            var message = new RequestMessageWithValidator(1, Enumerable.Range(0, 10));
            var errorInfo = message.Validate();

            Assert.IsNotNull(errorInfo);
            Assert.IsFalse(errorInfo.HasErrors);
        }

        [TestMethod]
        public void Validate_ReturnsExpectedErrors_IfMessageIsNotValid()
        {
            var message = new RequestMessageWithValidator(0, Enumerable.Empty<int>());
            var errorInfo = message.Validate();

            Assert.IsNotNull(errorInfo);
            Assert.IsTrue(errorInfo.HasErrors);            
            Assert.AreEqual(2, errorInfo.MemberErrors.Count);
            Assert.AreEqual("Error.", errorInfo.MemberErrors["IntValue"]);
            Assert.AreEqual("Error.", errorInfo.MemberErrors["IntValues"]);
        }

        #endregion        
    }
}
