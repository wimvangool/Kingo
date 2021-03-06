﻿using Kingo.Clocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MicroServices.Validation
{
    [TestClass]
    public sealed class CommandTest
    {
        #region [====== CommandUnderTest ======]

        private sealed class CommandUnderTest : CommandBase
        {
            private int _value;

            public CommandUnderTest(int value = 0)
            {
                _value = value;
            }

            public int Value
            {
                get => _value;
                set { SetValue(ref _value, value, () => Value); }
            }

            protected override IRequestMessageValidator CreateMessageValidator()
            {
                return base.CreateMessageValidator().Append<CommandUnderTest>((message, haltOnFirstError) =>
                {
                    var errorInfoBuilder = new ErrorInfoBuilder();

                    if (message.Value < 0)
                    {
                        errorInfoBuilder.Add("Error.", nameof(message.Value));
                    }
                    return errorInfoBuilder.BuildErrorInfo();
                });
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
            Assert.AreEqual(1, command.ErrorInfo.MemberErrors.Count);
            Assert.AreEqual("Error.", command.ErrorInfo.MemberErrors["Value"]);
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
            Assert.AreEqual(1, command.ErrorInfo.MemberErrors.Count);
            Assert.AreEqual("Error.", command.ErrorInfo.MemberErrors["Value"]);
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

        private static int RandomValue() =>
             Clock.Current.UtcDateAndTime().Millisecond;
    }
}
