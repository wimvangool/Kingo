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
           
            public static string ObjectValueLabel
            {
                get { return "[Object]"; }
            }

            private object _objectValue;

            [Required]           
            public object ObjectValue
            {
                get { return _objectValue; }
                set { SetValue(ref _objectValue, value, () => ObjectValue); }
            }

            #endregion

            #region [====== StringValueThatCannotBeEmpty ======]
            
            public static string StringValueThatCannotBeEmptyLabel
            {
                get { return "[StringThatCannotBeEmpty]"; }
            }

            private String _stringValueThatCannotBeEmpty;

            [Required(StringConstraint = RequiredStringConstraint.NotNullOrEmpty)]           
            public String StringValueThatCannotBeEmpty
            {
                get { return _stringValueThatCannotBeEmpty; }
                set { SetValue(ref _stringValueThatCannotBeEmpty, value, () => StringValueThatCannotBeEmpty); }
            }

            #endregion

            #region [====== StringValueThatCannotBeWhiteSpace ======]
            
            public static string StringValueThatCannotBeWhiteSpaceLabel
            {
                get { return "[StringThatCannotBeWhiteSpace]"; }
            }

            private String _stringValueThatCannotBeWhiteSpace;

            [Required(StringConstraint = RequiredStringConstraint.NotNullOrWhiteSpace)]
            public String StringValueThatCannotBeWhiteSpace
            {
                get { return _stringValueThatCannotBeWhiteSpace; }
                set { SetValue(ref _stringValueThatCannotBeWhiteSpace, value, () => StringValueThatCannotBeWhiteSpace); }
            }

            #endregion

            #region [====== NullableIntValue ======]

            public static string NullableIntValueLabel
            {
                get { return "[NullableIntValue]"; }
            }

            private int? _nullableIntValue;

            [Required]
            public int? NullableIntValue
            {
                get { return _nullableIntValue; }
                set { SetValue(ref _nullableIntValue, value, () => NullableIntValue); }
            }

            #endregion       
     
            #region [====== IntValue ======]

            public static string IntValueLabel
            {
                get { return "[IntValue]"; }
            }

            private int _intValue;

            [Required]
            public int IntValue
            {
                get { return _intValue; }
                set { SetValue(ref _intValue, value, () => IntValue); }
            }

            #endregion

            #region [====== GuidValue ======]

            public static string GuidValueLabel
            {
                get { return "[GuidValue]"; }
            }

            private Guid _guidValue;

            [Required]
            public Guid GuidValue
            {
                get { return _guidValue; }
                set { SetValue(ref _guidValue, value, () => GuidValue); }
            }

            #endregion
        }

        #endregion

        private readonly Random _random = new Random();
        private TestMessage _message;

        private IDataErrorInfo ErrorInfo
        {
            get { return _message; }
        }

        [TestInitialize]
        public void Setup()
        {
            _message = new TestMessage()
            {
                ObjectValue = new object(),
                StringValueThatCannotBeEmpty = "Something",
                StringValueThatCannotBeWhiteSpace = "Something Else",
                NullableIntValue = _random.Next(),
                IntValue = _random.Next(1, int.MaxValue),
                GuidValue = Guid.NewGuid()
            };            
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
        public void Message_IsMarkedInvalid_IfStringThatCannotBeEmptyIsEmpty()
        {
            _message.StringValueThatCannotBeEmpty = string.Empty;
            _message.Validate();

            Assert.IsFalse(_message.IsValid);
            AssertErrorMessage(ErrorInfo["StringValueThatCannotBeEmpty"], TestMessage.StringValueThatCannotBeEmptyLabel);
        }

        [TestMethod]
        public void Message_IsMarkedInvalid_IfStringThatCannotBeWhiteSpaceIsWhiteSpace()
        {
            _message.StringValueThatCannotBeWhiteSpace = new string(' ', _random.Next(1, 20));
            _message.Validate();

            Assert.IsFalse(_message.IsValid);
            AssertErrorMessage(ErrorInfo["StringValueThatCannotBeWhiteSpace"], TestMessage.StringValueThatCannotBeWhiteSpaceLabel);
        }

        [TestMethod]
        public void Message_IsMarkedInvalid_IfRequiredNullableIntValueIsNull()
        {
            _message.NullableIntValue = null;
            _message.Validate();

            Assert.IsFalse(_message.IsValid);
            AssertErrorMessage(ErrorInfo["NullableIntValue"], TestMessage.NullableIntValueLabel);
        }

        [TestMethod]
        public void Message_IsMarkedInvalid_IfRequiredIntValueIsZero()
        {
            _message.IntValue = 0;
            _message.Validate();

            Assert.IsFalse(_message.IsValid);
            AssertErrorMessage(ErrorInfo["IntValue"], TestMessage.IntValueLabel);
        }

        [TestMethod]
        public void Message_IsMarkedInvalid_IfRequiredGuidValueIsEmpty()
        {
            _message.GuidValue = Guid.Empty;
            _message.Validate();

            Assert.IsFalse(_message.IsValid);
            AssertErrorMessage(ErrorInfo["GuidValue"], TestMessage.GuidValueLabel);
        }

        private static void AssertErrorMessage(string errorMessage, string expectedLabel)
        {
            Assert.IsNotNull(errorMessage);
            Assert.IsTrue(errorMessage.Contains(expectedLabel));
        }
    }
}
