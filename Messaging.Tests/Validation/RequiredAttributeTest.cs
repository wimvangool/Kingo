using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Validation
{
    [TestClass]
    public sealed class RequiredAttributeTest
    {
        #region [====== TestMessage ======]

        private sealed class TestMessage : RequestMessage
        {
            public TestMessage() { }

            private TestMessage(TestMessage message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _objectValue = message._objectValue;
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new TestMessage(this, makeReadOnly);
            }

            #region [====== ObjectValue ======]

            [UsedImplicitly]
            public static string ObjectValueLabel
            {
                get { return "[Object]"; }
            }

            private object _objectValue;

            [Required]           
            public object ObjectValue
            {
                get { return _objectValue; }
                set
                {
                    if (_objectValue != value)
                    {
                        _objectValue = value;

                        NotifyOfPropertyChange(() => ObjectValue);
                    }
                }
            }

            #endregion

            #region [====== StringValue ======]

            [UsedImplicitly]
            public static string StringValueLabel
            {
                get { return "[String]"; }
            }

            private String _stringValue;

            [Required(StringConstraint = RequiredStringConstraint.NotNullOrEmpty)]           
            public String StringValue
            {
                get { return _stringValue; }
                set
                {
                    if (_stringValue != value)
                    {
                        _stringValue = value;

                        NotifyOfPropertyChange(() => StringValue);
                    }
                }
            }

            #endregion
        }

        #endregion

        private TestMessage _message;

        private IDataErrorInfo ErrorInfo
        {
            get { return _message; }
        }

        [TestInitialize]
        public void Setup()
        {
            _message = new TestMessage();
            _message.ObjectValue = new object();
            _message.StringValue = "Something";
        }

        [TestMethod]
        public void Message_IsMarkedValid_IfAllRequiredFieldsHaveCorrectValues()
        {            
            _message.Validate();

            Assert.IsTrue(_message.IsValid);
            Assert.IsNull(ErrorInfo.Error);
        }

        [TestMethod]
        public void Message_IsMarkedInvalid_IfRequiredObjectIsNull()
        {
            _message.ObjectValue = null;
            _message.Validate();

            Assert.IsFalse(_message.IsValid);            
            AssertErrorMessage(ErrorInfo["ObjectValue"], TestMessage.ObjectValueLabel);
        }

        [TestMethod]
        public void Message_IsMarkedInvalid_IfRequiredStringIsEmpty()
        {
            _message.StringValue = string.Empty;
            _message.Validate();

            Assert.IsFalse(_message.IsValid);
            AssertErrorMessage(ErrorInfo["StringValue"], TestMessage.StringValueLabel);
        }

        private static void AssertErrorMessage(string errorMessage, string expectedLabel)
        {
            Assert.IsNotNull(errorMessage);
            Assert.IsTrue(errorMessage.Contains(expectedLabel));
        }
    }
}
