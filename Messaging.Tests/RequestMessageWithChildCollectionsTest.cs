using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class RequestMessageWithChildCollectionsTest
    {
        #region [====== ParentMessage ======]

        private sealed class ParentMessage : RequestMessageViewModel<ParentMessage>
        {
            private readonly AttachedCollection<int> _integers;
            private readonly AttachedCollection<ChildMessage> _childMessages;

            public ParentMessage()
            {
                _integers = AttachCollection<int>();
                _childMessages = AttachCollection<ChildMessage>();
            }            

            private ParentMessage(ParentMessage message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _integers = AttachCollectionCopy(message._integers);
                _childMessages = AttachCollectionCopy(message._childMessages);
            }

            public override ParentMessage Copy(bool makeReadOnly)
            {
                return new ParentMessage(this, makeReadOnly);
            }            
            
            public AttachedCollection<int> Integers
            {
                get { return _integers; }                
            }            
            
            public AttachedCollection<ChildMessage> ChildMessages
            {
                get { return _childMessages; }
            }
        }

        #endregion

        #region [====== ChildMessage ======]

        private sealed class ChildMessage : RequestMessageViewModel<ChildMessage>
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

            public override ChildMessage Copy(bool makeReadOnly)
            {
                return new ChildMessage(this, makeReadOnly);
            }

            #region [====== Some Integer Value ======]

            private int _value;

            [RequiredConstraint]           
            public int Value
            {
                get { return _value; }
                set { SetValue(ref _value, value, () => Value); }
            }

            #endregion
        }

        #endregion

        #region [====== IntegerCollection ======]

        [TestMethod]
        public void Parent_IsMarkedAsChanged_WhenIntegerIsAdded()
        {
            var parent = new ParentMessage();

            parent.Validate();
            parent.Integers.Add(1);

            Assert.IsTrue(parent.HasChanges);
            Assert.IsTrue(parent.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Parent_Throws_WhenIntegerIsAddedToReadOnlyMessage()
        {
            var parent = new ParentMessage();

            parent.Validate();
            parent.Integers.Add(1);

            var copy = parent.Copy(true) as ParentMessage;

            Assert.IsNotNull(copy);
            Assert.AreNotSame(parent, copy);
            Assert.AreEqual(1, copy.Integers.Count);
            Assert.AreNotSame(parent.Integers, copy.Integers);

            copy.Integers.Add(2);
        }

        #endregion

        #region [====== ChildMessageCollection - ChangeTracking ======]

        [TestMethod]
        public void Parent_IsMarkedAsChanged_WhenAddedChildMessageChanges()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.ChildMessages.Add(child);
            parent.AcceptChanges();

            child.Value = 3;

            Assert.IsTrue(child.HasChanges);
            Assert.IsTrue(parent.HasChanges);
        }

        [TestMethod]
        public void ParentAndChild_AreNoLongerMarkedAsChanged_WhenAddedChildMessageHasAcceptedItChanges()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.ChildMessages.Add(child);
            parent.AcceptChanges();

            child.Value = 3;
            child.AcceptChanges();

            Assert.IsFalse(child.HasChanges);
            Assert.IsFalse(parent.HasChanges);
        }

        [TestMethod]
        public void ParentAndChild_AreNoLongerMarkedAsChanged_WhenParentHasAcceptedItsChanges()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.ChildMessages.Add(child);
            parent.AcceptChanges();

            child.Value = 3;
            parent.AcceptChanges();

            Assert.IsFalse(child.HasChanges);
            Assert.IsFalse(parent.HasChanges);
        }

        [TestMethod]
        public void Parent_IsNoLongerMarkedAsChanged_WhenRemovedChildChanges()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.ChildMessages.Add(child);
            parent.ChildMessages.Remove(child);
            parent.AcceptChanges();

            child.Value = 3;

            Assert.IsTrue(child.HasChanges);
            Assert.IsFalse(parent.HasChanges);
        }       
 
        [TestMethod]
        public void Child_IsNotMarkedAsChanged_WhenParentChanges()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.ChildMessages.Add(child);
            parent.AcceptChanges();

            parent.Integers.Add(5);

            Assert.IsTrue(parent.HasChanges);
            Assert.IsFalse(child.HasChanges);
        }
            
        #endregion

        #region [====== ChildCollection - Validation ======]

        [TestMethod]
        public void Parent_IsValid_WhenItSelfAndAllChildrenAreValid()
        {
            var parent = new ParentMessage();            

            parent.ChildMessages.Add(new ChildMessage(4));   
            parent.ChildMessages.Add(new ChildMessage(5));
            parent.Validate();

            Assert.IsTrue(parent.IsValid);            
        }

        [TestMethod]
        public void Parent_BecomesInvalid_WhenOneOfItsChildrenBecomesInvalid()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage(3);

            parent.ChildMessages.Add(child);
            parent.ChildMessages.Add(new ChildMessage(5));
            parent.Validate();            

            child.Value = 0;

            Assert.IsFalse(child.IsValid);
            Assert.IsFalse(parent.IsValid);
        }

        [TestMethod]
        public void Parent_BecomesValidAgain_WhenAllOfItsChildrenBecomeValidAgain()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage(3);

            parent.ChildMessages.Add(child);
            parent.ChildMessages.Add(new ChildMessage(5));
            parent.Validate();            

            child.Value = 0;
            child.Value = 1;

            Assert.IsTrue(child.IsValid);
            Assert.IsTrue(parent.IsValid);
        }

        [TestMethod]
        public void Parent_BecomesValidAgain_WhenInvalidChildIsRemoved()
        {
            var parent = new ParentMessage();
            var child = new ChildMessage();

            parent.ChildMessages.Add(child);
            parent.ChildMessages.Add(new ChildMessage(5));
            parent.Validate();

            parent.ChildMessages.Remove(child);

            Assert.IsFalse(child.IsValid);
            Assert.IsTrue(parent.IsValid);
        }

        #endregion
    }
}
