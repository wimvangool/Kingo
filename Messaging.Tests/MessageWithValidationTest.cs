using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging
{
    [TestClass]
    public sealed class MessageWithValidationTest
    {
        #region [====== MessageWithValidation ======]

        private sealed class MessageWithValidation : Message
        {
            private object _irrelevantValue;
            private Guid _customerId;
            private string _customerName;

            public MessageWithValidation() { }

            private MessageWithValidation(MessageWithValidation message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _irrelevantValue = message._irrelevantValue;
                _customerId = message._customerId;
                _customerName = message._customerName;
            }

            public object IrrelevantValue
            {
                get { return _irrelevantValue; }
                set
                {
                    if (_irrelevantValue != value)
                    {
                        _irrelevantValue = value;

                        OnPropertyChanged(() => IrrelevantValue, MessageChangedOption.None);
                    }
                }
            }

            public Guid CustomerId
            {
                get { return _customerId; }
                set
                {
                    if (_customerId != value)
                    {
                        _customerId = value;

                        OnPropertyChanged(() => CustomerId, MessageChangedOption.MarkAsChanged);
                    }
                }
            }

            [Required]
            public string CustomerName
            {
                get { return _customerName; }
                set
                {
                    if (_customerName != value)
                    {
                        _customerName = value;

                        OnPropertyChanged(() => CustomerName, MessageChangedOption.MarkAsChangedAndValidate);
                    }
                }
            }

            public override Message Copy(bool makeReadOnly)
            {
                return new MessageWithValidation(this, makeReadOnly);
            }
        }

        #endregion

        #region [====== Change Tracking ======]

        [TestMethod]
        public void Message_HasNoChanges_WhenJustCreated()
        {            
            var message = new MessageWithValidation();

            Assert.IsFalse(message.HasChanges);
        }

        [TestMethod]
        public void Message_HasNoChanges_AfterPropertyWithOptionNoneHasChanged()
        {
            bool eventWasRaised = false;

            var message = new MessageWithValidation();
            message.PropertyChanged += (s, e) =>
            {
                eventWasRaised = eventWasRaised || (e.PropertyName == "IrrelevantValue");
            };
            message.IrrelevantValue = new object();

            Assert.IsFalse(message.HasChanges);
            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_HasChanges_AfterPropertyWithOptionMarkAsChangedHasChanged()
        {
            bool eventWasRaised = false;

            var message = new MessageWithValidation();
            message.PropertyChanged += (s, e) =>
            {
                eventWasRaised = eventWasRaised || (e.PropertyName == "CustomerId");
            };
            message.CustomerId = Guid.NewGuid();

            Assert.IsTrue(message.HasChanges);
            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_HasChanges_AfterPropertyWithOptionMarkAsChangedAndValidateHasChanged()
        {
            bool eventWasRaised = false;

            var message = new MessageWithValidation();
            message.PropertyChanged += (s, e) =>
            {
                eventWasRaised = eventWasRaised || (e.PropertyName == "CustomerName");
            };
            message.CustomerName = "John Doe";

            Assert.IsTrue(message.HasChanges);
            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_RaisesHasChangesChangedEvent_WhenHasChangesChanges()
        {            
            bool eventWasRaised;

            var message = new MessageWithValidation();
            message.HasChangesChanged += (s, e) => eventWasRaised = true;

            // From false to false.
            eventWasRaised = false;
            message.HasChanges = false;

            Assert.IsFalse(eventWasRaised);

            // From false to true.
            message.HasChanges = true;

            Assert.IsTrue(eventWasRaised);

            // From true to true.
            eventWasRaised = false;
            message.HasChanges = true;

            Assert.IsFalse(eventWasRaised);

            // From true to false.
            eventWasRaised = false;
            message.HasChanges = false;

            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_RaisesPropertyChanged_WhenHasChangesChanges()
        {
            bool eventWasRaised;

            var message = new MessageWithValidation();
            message.PropertyChanged += (s, e) => eventWasRaised = (e.PropertyName == "HasChanges");

            // From false to false.
            eventWasRaised = false;
            message.HasChanges = false;

            Assert.IsFalse(eventWasRaised);

            // From false to true.
            message.HasChanges = true;

            Assert.IsTrue(eventWasRaised);

            // From true to true.
            eventWasRaised = false;
            message.HasChanges = true;

            Assert.IsFalse(eventWasRaised);

            // From true to false.
            eventWasRaised = false;
            message.HasChanges = false;

            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_Copy_ReturnsCopyThatHasNoChanges()
        {
            var message = new MessageWithValidation()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };

            Assert.IsTrue(message.HasChanges);

            var copy = message.Copy(false);

            Assert.IsNotNull(copy);
            Assert.IsFalse(copy.HasChanges);
        }

        [TestMethod]
        public void ReadOnlyMessage_CanChangeProperty_IfOptionIsNone()
        {
            var message = new MessageWithValidation()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };
            var copy = message.Copy(true) as MessageWithValidation;

            Assert.IsNotNull(copy);

            copy.IrrelevantValue = new object();            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyMessage_CannotChangeProperty_IfOptionIsMarkAsChanged()
        {
            var message = new MessageWithValidation()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };
            var copy = message.Copy(true) as MessageWithValidation;

            Assert.IsNotNull(copy);

            copy.CustomerId = Guid.NewGuid();            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyMessage_CannotChangeProperty_IfOptionIsMarkAsChangedAndValidate()
        {
            var message = new MessageWithValidation()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };
            var copy = message.Copy(true) as MessageWithValidation;

            Assert.IsNotNull(copy);

            copy.CustomerName = "Someone else";
        }

        #endregion

        #region [====== Validation ======]

        [TestMethod]
        public void Message_IsInvalid_WhenJustCreated()
        {
            var message = new MessageWithValidation();

            Assert.IsFalse(message.IsValid);
        }

        #endregion
    }
}
