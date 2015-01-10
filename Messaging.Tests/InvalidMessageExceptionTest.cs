using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class InvalidMessageExceptionTest
    {
        #region [====== ParentMessage ======]

        [Serializable]
        private sealed class ParentMessage : RequestMessageViewModel<ParentMessage>
        {
            private const string _ChildMessagesKey = "_childMessages";            
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
                _child = AttachCopy(message._child);
                _childMessages = AttachCollectionCopy(message._childMessages);
            }

            private ParentMessage(SerializationInfo info, StreamingContext context)           
            {
                _value = info.GetInt32(_ValueKey);
                _child = Attach<ChildMessage>(info, _ChildKey);
                _childMessages = AttachCollection<ChildMessage>(info, _ChildMessagesKey);
            }

            protected override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue(_ValueKey, _value);
                info.AddValue(_ChildKey, _child);
                info.AddCollection(_ChildMessagesKey, _childMessages);
            }

            public override ParentMessage Copy(bool makeReadOnly)
            {
                return new ParentMessage(this, makeReadOnly);
            }

            #region [====== Some Integer Value ======]

            private const string _ValueKey = "_value";            
            private int _value;

            [RequiredConstraint]
            public int Value
            {
                get { return _value; }
                set { SetValue(ref _value, value, () => Value); }
            }

            #endregion

            #region [====== ChildMessages ======]

            private const string _ChildKey = "_child";            
            private ChildMessage _child;

            public ChildMessage Child
            {
                get { return _child; }
                set { SetValue(ref _child, value, () => Child); }
            }

            public ObservableCollection<ChildMessage> ChildMessages
            {
                get { return _childMessages; }
            }

            #endregion            
        }

        #endregion

        #region [====== ChildMessage ======]

        [Serializable]
        private sealed class ChildMessage : RequestMessageViewModel<ChildMessage>
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

            private ChildMessage(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                _value = info.GetInt32(_ValueKey);
            }

            public override ChildMessage Copy(bool makeReadOnly)
            {
                return new ChildMessage(this, makeReadOnly);
            }

            protected override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);

                info.AddValue(_ValueKey, _value);
            }

            #region [====== Some Integer Value ======]

            private const string _ValueKey = "_value";
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
        public void Exception_IsCorrectlySerializedAndDeserialized_WithBinaryFormatter()
        {
            var message = new ParentMessage(0)
            {
                Child = new ChildMessage()
            };
            message.ChildMessages.Add(new ChildMessage(2));
            message.ChildMessages.Add(new ChildMessage());
            message.ChildMessages.Add(new ChildMessage(4));
            message.AcceptChanges();

            var validator = message as IMessage<ParentMessage>;
            ValidationErrorTree errorTree;
            
            Assert.IsTrue(validator.TryGetValidationErrors(out errorTree));
            Assert.IsNotNull(errorTree);

            var exception = new InvalidMessageException(message, "Message contains errors.", errorTree);
            var exceptionCopy = CopyThroughSerialization(exception);

            Assert.IsNotNull(exceptionCopy);
            Assert.AreNotSame(exception, exceptionCopy);
            AssertEqualParentMessage(message, exceptionCopy.FailedMessage as ParentMessage);
            AssertEqualErrorTree(errorTree, exceptionCopy.ErrorTree);
        }

        private static InvalidMessageException CopyThroughSerialization(InvalidMessageException exception)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, exception);
                stream.Position = 0;

                return (InvalidMessageException) formatter.Deserialize(stream);
            }
        }

        private static void AssertEqualParentMessage(ParentMessage message, ParentMessage messageCopy)
        {
            Assert.IsNotNull(messageCopy);
            Assert.AreNotSame(message, messageCopy);
            Assert.AreEqual(message.Value, messageCopy.Value);
            AssertEqualChildMessage(message.Child, messageCopy.Child);

            for (int index = 0; index < message.ChildMessages.Count; index++)
            {
                AssertEqualChildMessage(message.ChildMessages[index], messageCopy.ChildMessages[index]);
            }
        }

        private static void AssertEqualChildMessage(ChildMessage message, ChildMessage messageCopy)
        {
            Assert.AreEqual(message.Value, messageCopy.Value);
        }

        private static void AssertEqualErrorTree(ValidationErrorTree errorTree, ValidationErrorTree errorTreeCopy)
        {
            Assert.IsNotNull(errorTreeCopy);
            Assert.AreSame(errorTree.MessageType, errorTreeCopy.MessageType);
            Assert.AreEqual(errorTree.TotalErrorCount, errorTreeCopy.TotalErrorCount);

            foreach (var error in errorTree.Errors)
            {
                string errorMessage;

                Assert.IsTrue(errorTreeCopy.Errors.TryGetValue(error.Key, out errorMessage));
                Assert.AreEqual(error.Value, errorMessage);
            }
            var childErrors = errorTree.ChildErrors.ToArray();
            var childErrorsCopy = errorTreeCopy.ChildErrors.ToArray();

            for (int index = 0; index < childErrors.Length; index++)
            {
                AssertEqualErrorTree(childErrors[index], childErrorsCopy[index]);
            }
        }        
    }
}
