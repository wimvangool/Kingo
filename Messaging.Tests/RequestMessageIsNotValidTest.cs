using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class RequestMessageIsNotValidTest
    {
        #region [====== ParentMessage ======]

        private sealed class ParentMessage : RequestMessage<ParentMessage>
        {
            private readonly ObservableCollection<ChildMessage> _childMessages;            

            public ParentMessage(int value)
            {
                _value = value;
                _childMessages = AttachCollection<ChildMessage>();
            }

            private ParentMessage(ParentMessage message, bool makeReadOnly)
                : base(message, makeReadOnly)
            {
                _value = message._value;
                _childMessages = AttachCollectionCopy(message._childMessages);
            }

            public override ParentMessage Copy(bool makeReadOnly)
            {
                return new ParentMessage(this, makeReadOnly);
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

            #region [====== ChildMessage ======]

            private ChildMessage _child;

            public ChildMessage Child
            {
                get { return _child; }
                set { SetValue(ref _child, value, () => Child); }
            }

            #endregion

            #region [====== ChildMessageCollection ======]

            public ObservableCollection<ChildMessage> ChildMessages
            {
                get { return _childMessages; }
            }

            #endregion
        }

        #endregion

        #region [====== ChildMessage ======]

        private sealed class ChildMessage : RequestMessage<ChildMessage>
        {
            public ChildMessage() { }

            public ChildMessage(int value)
            {
                _value = value;
            }

            private ChildMessage(ChildMessage message, bool makeReadOnly)
                : base(message, makeReadOnly)
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

        [TestMethod]
        public void IsNotValid_ReturnsFalse_IfMessageIsValid()
        {
            var message = new ParentMessage(1);
            var validator = message as IMessageValidator<ParentMessage>;
            MessageErrorTree errorTree;

            Assert.IsFalse(validator.IsNotValid(message, out errorTree));
            Assert.IsNull(errorTree);
            Assert.IsTrue(message.IsValid);
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfMessagePropertyIsNotValid()
        {
            var message = new ParentMessage(0);
            var validator = message as IMessageValidator<ParentMessage>;
            MessageErrorTree errorTree;

            Assert.IsTrue(validator.IsNotValid(message, out errorTree));
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(typeof(ParentMessage), errorTree.MessageType);
            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.IsTrue(errorTree.Errors.ContainsKey("Value"));
        }     
   
        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfChildMessageIsNotValid()
        {
            var message = new ParentMessage(1)
            {
                Child = new ChildMessage()
            };
            var validator = message as IMessageValidator<ParentMessage>;
            MessageErrorTree errorTree;

            Assert.IsTrue(validator.IsNotValid(message, out errorTree));
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(typeof(ParentMessage), errorTree.MessageType);
            Assert.AreEqual(1, errorTree.TotalErrorCount);
            Assert.AreEqual(1, errorTree.ChildErrors.Count());

            var childErrorTree = errorTree.ChildErrors.First();
            
            Assert.AreEqual(typeof(ChildMessage), childErrorTree.MessageType);
            Assert.AreEqual(1, childErrorTree.TotalErrorCount);
            Assert.IsTrue(childErrorTree.Errors.ContainsKey("Value"));
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfChildMessagesInCollectionAreNotValid()
        {
            var message = new ParentMessage(1);
            message.ChildMessages.Add(new ChildMessage());
            message.ChildMessages.Add(new ChildMessage());

            var validator = message as IMessageValidator<ParentMessage>;
            MessageErrorTree errorTree;

            Assert.IsTrue(validator.IsNotValid(message, out errorTree));
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(typeof(ParentMessage), errorTree.MessageType);
            Assert.AreEqual(2, errorTree.TotalErrorCount);
            Assert.AreEqual(1, errorTree.ChildErrors.Count());

            var childErrorTree = errorTree.ChildErrors.First();

            Assert.AreEqual(typeof(ObservableCollection<ChildMessage>), childErrorTree.MessageType);
            Assert.AreEqual(2, childErrorTree.TotalErrorCount);
            Assert.AreEqual(2, childErrorTree.ChildErrors.Count());

            var childErrorTreeOne = childErrorTree.ChildErrors.ElementAt(0);

            Assert.IsNotNull(childErrorTreeOne);
            Assert.AreEqual(typeof(ChildMessage), childErrorTreeOne.MessageType);
            Assert.AreEqual(1, childErrorTreeOne.TotalErrorCount);
            Assert.IsTrue(childErrorTreeOne.Errors.ContainsKey("Value"));

            var childErrorTreeTwo = childErrorTree.ChildErrors.ElementAt(1);

            Assert.IsNotNull(childErrorTreeTwo);
            Assert.AreEqual(typeof(ChildMessage), childErrorTreeTwo.MessageType);
            Assert.AreEqual(1, childErrorTreeTwo.TotalErrorCount);
            Assert.IsTrue(childErrorTreeTwo.Errors.ContainsKey("Value"));
        }

        [TestMethod]
        public void IsNotValid_ReturnsTrue_IfAnyMessageIsNotValid()
        {
            var message = new ParentMessage(0)
            {
                Child = new ChildMessage(1)
            };
            message.ChildMessages.Add(new ChildMessage());
            message.ChildMessages.Add(new ChildMessage(1));
            message.ChildMessages.Add(new ChildMessage());

            var validator = message as IMessageValidator<ParentMessage>;
            MessageErrorTree errorTree;

            Assert.IsTrue(validator.IsNotValid(message, out errorTree));
            Assert.IsNotNull(errorTree);
            Assert.AreEqual(typeof(ParentMessage), errorTree.MessageType);
            Assert.AreEqual(3, errorTree.TotalErrorCount);
            Assert.AreEqual(1, errorTree.ChildErrors.Count());
            Assert.IsTrue(errorTree.Errors.ContainsKey("Value"));

            var childErrorTree = errorTree.ChildErrors.First();

            Assert.AreEqual(typeof(ObservableCollection<ChildMessage>), childErrorTree.MessageType);
            Assert.AreEqual(2, childErrorTree.TotalErrorCount);
            Assert.AreEqual(2, childErrorTree.ChildErrors.Count());

            var childErrorTreeOne = childErrorTree.ChildErrors.ElementAt(0);

            Assert.IsNotNull(childErrorTreeOne);
            Assert.AreEqual(typeof(ChildMessage), childErrorTreeOne.MessageType);
            Assert.AreEqual(1, childErrorTreeOne.TotalErrorCount);
            Assert.IsTrue(childErrorTreeOne.Errors.ContainsKey("Value"));

            var childErrorTreeTwo = childErrorTree.ChildErrors.ElementAt(1);

            Assert.IsNotNull(childErrorTreeTwo);
            Assert.AreEqual(typeof(ChildMessage), childErrorTreeTwo.MessageType);
            Assert.AreEqual(1, childErrorTreeTwo.TotalErrorCount);
            Assert.IsTrue(childErrorTreeTwo.Errors.ContainsKey("Value"));
        }
    }
}
