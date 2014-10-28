using System.ComponentModel.Messaging.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging
{
    [TestClass]
    public sealed class RequestMessageWithChildMessageTest
    {
        #region [====== ParentMessage ======]

        private sealed class ParentMessage : RequestMessage
        {
            public ParentMessage() { }

            public ParentMessage(int value)
            {
                _value = value;
            }

            private ParentMessage(ParentMessage message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _value = message._value;
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new ParentMessage(this, makeReadOnly);
            }

            #region [====== Some Integer Value ======]

            private int _value;

            [Required]
            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public int Value
            {
                get { return _value; }
                set { SetValue(ref _value, value, () => Value); }
            }

            #endregion

            #region [====== ChildMessage ======]

            private ChildMessage _child;

            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public ChildMessage Child
            {
                get { return _child; }
                set { SetValue(ref _child, value, () => Child); }
            }

            #endregion
        }

        #endregion

        #region [====== ChildMessage ======]

        private sealed class ChildMessage : RequestMessage
        {
            public ChildMessage() { }

            public ChildMessage(int value)
            {
                _value = value;
            }

            private ChildMessage(ChildMessage message, bool makeReadOnly): base(message, makeReadOnly)
            {
                _value = message._value;
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new ChildMessage(this, makeReadOnly);
            }

            #region [====== Some Integer Value ======]

            private int _value;

            [Required]
            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public int Value
            {
                get { return _value; }
                set { SetValue(ref _value, value, () => Value); }
            }

            #endregion
        }

        #endregion

        #region [====== GetMessage and SetMessage =====]

        [TestMethod]
        public void GetMessage_ReturnsNull_IfNoValueWasSetBefore()
        {
            var parent = new ParentMessage();

            Assert.IsNull(parent.Child);
        }

        [TestMethod]
        public void GetMessage_ReturnsNull_IfValueWasSetToNull()
        {
            var parent = new ParentMessage();

            parent.Child = null;

            Assert.IsNull(parent.Child);
        }

        [TestMethod]
        public void GetMessage_ReturnsChildInstance_IfValueWasSetToInstance()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.Child = child;

            Assert.AreSame(child, parent.Child);
        }

        [TestMethod]
        public void GetMessage_ReturnsNewChildInstance_IfValueWasOverriddenByNewValue()
        {
            var parent = new ParentMessage();
            var childA = new ChildMessage();
            var childB = new ChildMessage();

            parent.Child = childA;
            parent.Child = childB;

            Assert.AreSame(childB, parent.Child);
        }

        #endregion

        #region [====== Change Tracking ======]

        [TestMethod]
        public void Parent_IsMarkedAsChanged_WhenChildIsSetToAnotherInstance()
        {
            var parent = new ParentMessage();

            Assert.IsFalse(parent.HasChanges);

            parent.Child = new ChildMessage();

            Assert.IsTrue(parent.HasChanges);
        }

        [TestMethod]
        public void Parent_IsMarkedAsChanged_WhenChildChanges()
        {
            var parent = new ParentMessage()
            {
                Child = new ChildMessage()
            };
            parent.AcceptChanges();
            parent.Child.Value = 1;

            Assert.IsTrue(parent.Child.HasChanges);
            Assert.IsTrue(parent.HasChanges);
        }        

        [TestMethod]
        public void Child_IsMarkedAsUnchanged_WhenParentIsMarkedAsUnchanged()
        {
            var parent = new ParentMessage()
            {
                Child = new ChildMessage()
            };
            parent.AcceptChanges();
            parent.Child.Value = 1;
            parent.AcceptChanges();

            Assert.IsFalse(parent.HasChanges);
            Assert.IsFalse(parent.Child.HasChanges);            
        }

        [TestMethod]
        public void Parent_IsMarkedUnchanged_WhenChildChangesAreAccepted()
        {
            var parent = new ParentMessage()
            {
                Child = new ChildMessage()
            };
            parent.AcceptChanges();
            parent.Child.Value = 1;
            parent.Child.AcceptChanges();

            Assert.IsFalse(parent.HasChanges);
            Assert.IsFalse(parent.Child.HasChanges); 
        }

        [TestMethod]
        public void Parent_IsStillMarkedAsChanged_WhenOnlyChildChangesAreAccepted()
        {
            var parent = new ParentMessage()
            {
                Child = new ChildMessage()
            };            

            parent.Child.Value = 1;
            parent.Child.AcceptChanges();

            Assert.IsTrue(parent.HasChanges);
            Assert.IsFalse(parent.Child.HasChanges); 
        }

        [TestMethod]
        public void Parent_IsNoLongerMarkedAsChanged_WhenChildThatIsSwitchedOutChanges()
        {
            var parent = new ParentMessage();
            var childA = new ChildMessage();
            var childB = new ChildMessage();

            parent.Child = childA;
            parent.Child = childB;
            parent.AcceptChanges();

            childA.Value = 4;

            Assert.IsFalse(parent.HasChanges);
            Assert.IsTrue(childA.HasChanges);
            Assert.IsFalse(childB.HasChanges);
        }

        #endregion

        #region [====== Validation ======]

        [TestMethod]
        public void Child_IsValidated_WhenParentIsValidated()
        {
            var parent = new ParentMessage(2);
            var child = new ChildMessage(4);

            Assert.IsFalse(parent.IsValid);           
            Assert.IsFalse(child.IsValid);

            // Triggers validation of parent.
            parent.Child = child;

            Assert.IsTrue(parent.IsValid);
            Assert.IsTrue(child.IsValid);
        }

        [TestMethod]
        public void Parent_IsMarkedInvalid_WhenChildIsMarkedInvalid()
        {
            var parent = new ParentMessage(3)
            {
                Child = new ChildMessage(4)
            };

            Assert.IsTrue(parent.IsValid);
            Assert.IsTrue(parent.Child.IsValid);

            // Marks the child as invalid.
            parent.Child.Value = 0;

            Assert.IsFalse(parent.IsValid);
            Assert.IsFalse(parent.Child.IsValid);
        }

        [TestMethod]
        public void Parent_IsNotLongerMarkedInvalid_WhenChildThatIsSwitchedOutBecomesInvalid()
        {
            var parent = new ParentMessage(3);
            var child = new ChildMessage(2);

            parent.Child = child;            

            Assert.IsTrue(parent.IsValid);
            Assert.IsTrue(parent.Child.IsValid);

            parent.Child = new ChildMessage(6);

            Assert.IsTrue(parent.IsValid);
            Assert.IsTrue(parent.Child.IsValid);

            child.Value = 0;

            Assert.IsTrue(parent.IsValid);
            Assert.IsTrue(parent.Child.IsValid);
            Assert.IsFalse(child.IsValid);
        }

        #endregion
    }
}
