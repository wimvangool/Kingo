using System.ComponentModel.Messaging.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging
{
    [TestClass]
    public sealed class RequestMessagePropertyTest
    {
        #region [====== MessageWithValidation ======]

        private sealed class TestMessage : RequestMessage
        {
            private object _irrelevantValue;
            private Guid _customerId;
            private string _customerName;

            public TestMessage() { }

            private TestMessage(TestMessage message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _irrelevantValue = message._irrelevantValue;
                _customerId = message._customerId;
                _customerName = message._customerName;
            }

            [RequestMessageProperty(PropertyChangedOption.None)]
            public object IrrelevantValue
            {
                get { return _irrelevantValue; }
                set { SetValue(() => IrrelevantValue, value, ref _irrelevantValue); }
            }

            [RequestMessageProperty(PropertyChangedOption.MarkAsChanged)]
            public Guid CustomerId
            {
                get { return _customerId; }
                set { SetValue(() => CustomerId, value, ref _customerId); }                                                   
            }

            [Required]
            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public string CustomerName
            {
                get { return _customerName; }
                set { SetValue(() => CustomerName, value, ref _customerName); }
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new TestMessage(this, makeReadOnly);
            }
        }

        #endregion

        #region [====== Change Tracking ======]

        [TestMethod]
        public void Message_HasNoChanges_WhenJustCreated()
        {            
            var message = new TestMessage();

            Assert.IsFalse(message.HasChanges);
        }

        [TestMethod]
        public void Message_HasNoChanges_AfterPropertyWithOptionNoneHasChanged()
        {
            bool eventWasRaised = false;

            var message = new TestMessage();
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

            var message = new TestMessage();
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

            var message = new TestMessage();
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

            var message = new TestMessage();
            message.HasChangesChanged += (s, e) => eventWasRaised = true;

            // From false to false.
            eventWasRaised = false;
            message.AcceptChanges();

            Assert.IsFalse(eventWasRaised);

            // From false to true.
            message.CustomerId = Guid.NewGuid();

            Assert.IsTrue(eventWasRaised);            

            // From true to false.
            eventWasRaised = false;
            message.AcceptChanges();

            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_RaisesPropertyChanged_WhenHasChangesChanges()
        {
            bool eventWasRaised;

            var message = new TestMessage();
            message.PropertyChanged += (s, e) => eventWasRaised = (e.PropertyName == "HasChanges");

            // From false to false.
            eventWasRaised = false;
            message.AcceptChanges();

            Assert.IsFalse(eventWasRaised);

            // From false to true.
            message.CustomerId = Guid.NewGuid();

            Assert.IsTrue(eventWasRaised);            

            // From true to false.
            eventWasRaised = false;
            message.AcceptChanges();

            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void Message_Copy_ReturnsCopyThatHasNoChanges()
        {
            var message = new TestMessage()
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
            var message = new TestMessage()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };
            var copy = message.Copy(true) as TestMessage;

            Assert.IsNotNull(copy);

            copy.IrrelevantValue = new object();            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyMessage_CannotChangeProperty_IfOptionIsMarkAsChanged()
        {
            var message = new TestMessage()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };
            var copy = message.Copy(true) as TestMessage;

            Assert.IsNotNull(copy);

            copy.CustomerId = Guid.NewGuid();            
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadOnlyMessage_CannotChangeProperty_IfOptionIsMarkAsChangedAndValidate()
        {
            var message = new TestMessage()
            {
                IrrelevantValue = new object(),
                CustomerId = Guid.NewGuid(),
                CustomerName = "Bob"
            };
            var copy = message.Copy(true) as TestMessage;

            Assert.IsNotNull(copy);

            copy.CustomerName = "Someone else";
        }

        #endregion

        #region [====== Validation ======]

        [TestMethod]
        public void Message_IsInvalid_WhenJustCreated()
        {
            var message = new TestMessage();

            Assert.IsFalse(message.IsValid);
        }

        [TestMethod]
        public void Message_IsValidated_WhenPropertyWithOptionValidateHasChanged()
        {
            bool eventWasRaised = false;

            var message = new TestMessage();

            message.PropertyChanged += (s, e) =>
            {
                eventWasRaised = eventWasRaised || (e.PropertyName == "IsValid");
            };
            message.CustomerName = "Some Customer";

            Assert.IsTrue(eventWasRaised);
            Assert.IsTrue(message.IsValid);
        }



        #endregion
    }
}
