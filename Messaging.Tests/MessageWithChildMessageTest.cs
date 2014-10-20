using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging
{
    [TestClass]
    public sealed class MessageWithChildMessageTest
    {
        #region [====== ParentMessage ======]

        private sealed class ParentMessage : RequestMessage
        {
            public ParentMessage() { }

            private ParentMessage(ParentMessage message, bool makeReadOnly)
                : base(message, makeReadOnly)
            {
                _value = message._value;
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new ParentMessage(this, makeReadOnly);
            }

            #region [====== Some Integer Value ======]

            private int _value;

            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public int Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;

                        NotifyOfPropertyChange(() => Value);
                    }
                }
            }

            #endregion

            #region [====== ChildMessage ======]

            [RequestMessageProperty(PropertyChangedOption.MarkAsChanged)]
            public ChildMessage Child
            {
                get { return GetMessage(() => Child); }
                set { SetMessage(() => Child, value); }
            }

            #endregion
        }

        #endregion

        #region [====== ChildMessage ======]

        private sealed class ChildMessage : RequestMessage
        {
            public ChildMessage() { }

            private ChildMessage(ChildMessage message, bool makeReadOnly)
                : base(message, makeReadOnly)
            {
                _value = message._value;
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new ChildMessage(this, makeReadOnly);
            }

            #region [====== Some Integer Value ======]

            private int _value;

            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public int Value
            {
                get { return _value; }
                set
                {
                    if (_value != value)
                    {
                        _value = value;

                        NotifyOfPropertyChange(() => Value);
                    }
                }
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

        #endregion

        #region [====== Validation ======]



        #endregion
    }
}
