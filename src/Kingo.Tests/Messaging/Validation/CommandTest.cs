using Kingo.Clocks;
using Kingo.Messaging.Validation.Constraints;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Messaging.Validation
{
    [TestClass]
    public sealed class CommandTest
    {
        #region [====== CommandUnderTest ======]

        private sealed class CommandUnderTest : Command
        {
            private int _value;

            public CommandUnderTest(int value = 0)
            {
                _value = value;
            }

            public int Value
            {
                get { return _value; }
                set { SetValue(ref _value, value, () => Value); }
            }

            protected override ValidateMethod Implement(ValidateMethod method) =>
                base.Implement(method).Add(this, CreateValidator, true);

            private static IMessageValidator<CommandUnderTest> CreateValidator()
            {
                var validator = new ConstraintMessageValidator<CommandUnderTest>();
                validator.VerifyThat(m => m.Value).IsGreaterThanOrEqualTo(0);
                return validator;
            }
        }

        #endregion

        #region [====== HasChanged ======]

        [TestMethod]
        public void HasChanges_IsFalse_WhenCommandIsJustCreated()
        {
            Assert.IsFalse(new CommandUnderTest().HasChanges);
        }

        [TestMethod]
        public void HasChanges_IsNotChanged_WhenNewValueIsEqualToOldValue()
        {
            var value = RandomValue();            
            var command = new CommandUnderTest(value);
            
            command.Value = value;

            Assert.IsFalse(command.HasChanges);
        }

        [TestMethod]
        public void HasChanges_IsChanged_WhenNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();            
            var command = new CommandUnderTest(value);
            
            command.Value = value + 1;
            
            Assert.IsTrue(command.HasChanges);
        }

        [TestMethod]
        public void AcceptChanges_DoesNothing_IfHasChangesIsAlreadyFalse()
        {
            var wasRaised = false;
            var command = new CommandUnderTest();

            command.PropertyChanged += (s, e) => wasRaised = true;
            command.AcceptChanges();

            Assert.IsFalse(wasRaised);
            Assert.IsFalse(command.HasChanges);
        }

        [TestMethod]
        public void AcceptChanges_SetsHasChangesToFalse_IfHasChangesIsTrue()
        {
            var wasRaised = false;
            var command = new CommandUnderTest();

            command.Value = RandomValue() + 1;

            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanges")
                {
                    wasRaised = true;
                }
            };
            command.AcceptChanges();

            Assert.IsTrue(wasRaised);
            Assert.IsFalse(command.HasChanges);
        }

        #endregion

        #region [====== ErrorInfo ======]

        [TestMethod]
        public void HasErrors_IsFalse_WhenCommandIsJustCreated_And_CommandIsValid()
        {
            Assert.IsFalse(new CommandUnderTest().HasErrors);
        }

        [TestMethod]
        public void HasErrors_IsTrue_WhenCommandIsJustCreated_And_CommandIsNotValid()
        {
            Assert.IsTrue(new CommandUnderTest(-1).HasErrors);
        }

        [TestMethod]
        public void ErrorInfo_IsEmpty_WhenCommandIsJustCreated_And_CommandIsValid()
        {
            var command = new CommandUnderTest();

            Assert.IsNotNull(command.ErrorInfo);
            Assert.IsFalse(command.ErrorInfo.HasErrors);
        }

        [TestMethod]
        public void ErrorInfo_ContainsExpectedErrors_WhenCommandIsJustCreated_And_CommandIsNotValid()
        {
            var command = new CommandUnderTest(-1);

            Assert.IsNotNull(command.ErrorInfo);
            Assert.AreEqual(2, command.ErrorInfo.ErrorCount);
            Assert.AreEqual("Value (-1) must be greater than or equal to '0'.", command.ErrorInfo.MemberErrors["Value"]);
        }

        [TestMethod]
        public void ErrorInfo_IsEmpty_WhenPropertyChangesFromInvalidToValid()
        {
            var command = new CommandUnderTest(-1);

            command.Value = 0;

            Assert.IsNotNull(command.ErrorInfo);
            Assert.IsFalse(command.ErrorInfo.HasErrors);
        }

        [TestMethod]
        public void ErrorInfo_ContainsExpectedErrors_WhenPropertyChangedFromValidToInvalid()
        {
            var command = new CommandUnderTest();

            command.Value = -1;

            Assert.IsNotNull(command.ErrorInfo);
            Assert.AreEqual(2, command.ErrorInfo.ErrorCount);
            Assert.AreEqual("Value (-1) must be greater than or equal to '0'.", command.ErrorInfo.MemberErrors["Value"]);
        }

        #endregion

        #region [====== PropertyChanging ======]

        [TestMethod]
        public void PropertyChanging_IsNotRaised_IfNewValueIsEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanging += (s, e) => wasRaised = true;
            command.Value = value;

            Assert.IsFalse(wasRaised);
        }

        [TestMethod]
        public void PropertyChanging_IsRaised_ForProperty_IfNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanging += (s, e) =>
            {
                if (e.PropertyName == "Value")
                {
                    wasRaised = true;
                }
            };
            command.Value = value + 1;

            Assert.IsTrue(wasRaised);
        }

        [TestMethod]
        public void PropertyChanging_IsRaised_ForHasChanges_IfNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanging += (s, e) =>
            {
                if (e.PropertyName == "HasChanges")
                {
                    wasRaised = true;
                }
            };
            command.Value = value + 1;

            Assert.IsTrue(wasRaised);
        }

        [TestMethod]
        public void PropertyChanging_IsRaised_ForErrorInfo_IfNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanging += (s, e) =>
            {
                if (e.PropertyName == "ErrorInfo")
                {
                    wasRaised = true;
                }
            };
            command.Value = value + 1;

            Assert.IsTrue(wasRaised);
        }

        #endregion

        #region [====== PropertyChanged ======]

        [TestMethod]
        public void PropertyChanged_IsNotRaised_IfNewValueIsEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanged += (s, e) => wasRaised = true;
            command.Value = value;

            Assert.IsFalse(wasRaised);
        }

        [TestMethod]
        public void PropertyChanged_IsRaised_ForProperty_IfNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Value")
                {
                    wasRaised = true;
                }
            };
            command.Value = value + 1;

            Assert.IsTrue(wasRaised);
        }

        [TestMethod]
        public void PropertyChanged_IsRaised_ForHasChanges_IfNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasChanges")
                {
                    wasRaised = true;
                }
            };
            command.Value = value + 1;

            Assert.IsTrue(wasRaised);
        }

        [TestMethod]
        public void PropertyChanged_IsRaised_ForErrorInfo_IfNewValueIsNotEqualToOldValue()
        {
            var value = RandomValue();
            var wasRaised = false;
            var command = new CommandUnderTest(value);

            command.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "ErrorInfo")
                {
                    wasRaised = true;
                }
            };
            command.Value = value + 1;

            Assert.IsTrue(wasRaised);
        }

        #endregion

        private static int RandomValue()
        {
            return Clock.Current.UtcDateAndTime().Millisecond;
        }
    }
}
