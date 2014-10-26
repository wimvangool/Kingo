using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging
{
    [TestClass]
    public sealed class RequestMessageAsEditableObjectTest
    {
        #region [====== TestMessage ======]

        private sealed class TestMessage : RequestMessage
        {
            public TestMessage() { }

            private TestMessage(TestMessage message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _intValue = message._intValue;
                _child = AttachCopy(message._child);
            }

            public override RequestMessage Copy(bool makeReadOnly)
            {
                return new TestMessage(this, makeReadOnly);
            }            

            private int _intValue;

            [Required]
            [RequestMessageProperty(PropertyChangedOption.MarkAsChangedAndValidate)]
            public int IntValue
            {
                get { return _intValue; }
                set { SetValue(ref _intValue, value, () => IntValue); }
            }

            private TestMessage _child;
            
            [RequestMessageProperty(PropertyChangedOption.MarkAsChanged)]
            public TestMessage Child
            {
                get { return _child; }
                set { SetValue(ref _child, value, () => Child); }
            }            
        }

        #endregion

        private TestMessage _message;

        private IEditableObject EditableMessage
        {
            get { return _message; }
        }

        [TestInitialize]
        public void Setup()
        {
            _message = new TestMessage();
        }

        #region [====== BeginEdit, CancelEdit and EndEdit ======]

        [TestMethod]
        public void Message_IsNotInEditMode_ByDefault()
        {
            Assert.IsFalse(_message.IsInEditMode);
        }

        [TestMethod]
        public void BeginEdit_CanBeCalledMultipleTimes()
        {
            EditableMessage.BeginEdit();
            EditableMessage.BeginEdit();
        }

        [TestMethod]
        public void BeginEdit_BringsMessageIntoEditMode()
        {
            var eventWasRaised = false;

            _message.PropertyChanged += (s, e) => eventWasRaised = e.PropertyName == "IsInEditMode";

            EditableMessage.BeginEdit();

            Assert.IsTrue(eventWasRaised);
            Assert.IsTrue(_message.IsInEditMode);
        }

        [TestMethod]
        public void BeginEdit_DoesNotPreventValidationToExecute()
        {
            var eventWasRaised = false;

            _message.IsValidChanged += (s, e) => eventWasRaised = true;
           
            EditableMessage.BeginEdit();

            _message.IntValue = 6;
           
            Assert.IsTrue(eventWasRaised);
            Assert.IsTrue(_message.IsValid);
        }

        [TestMethod]
        public void CancelEdit_TakesMessageOutOfEditMode()
        {
            var eventWasRaised = false;

            EditableMessage.BeginEdit();

            _message.PropertyChanged += (s, e) => eventWasRaised = e.PropertyName == "IsInEditMode";

            EditableMessage.CancelEdit();

            Assert.IsTrue(eventWasRaised);
            Assert.IsFalse(_message.IsInEditMode);
        }

        [TestMethod]
        public void CancelEdit_RollsBackAnyChangesMade()
        {
            EditableMessage.BeginEdit();

            _message.IntValue = 4;
            _message.Child = new TestMessage();

            EditableMessage.CancelEdit();

            Assert.AreEqual(0, _message.IntValue);
            Assert.IsNull(_message.Child);
        }

        [TestMethod]
        public void CancelEdit_CanBeCalledOutsideEditScope_WithoutAnySideEffects()
        {
            EditableMessage.CancelEdit();

            Assert.IsFalse(_message.IsInEditMode);
        }

        [TestMethod]
        public void CancelEdit_CanBeCalledMultipleTimes_WithoutAnySideEffects()
        {
            EditableMessage.BeginEdit();
            EditableMessage.CancelEdit();
            EditableMessage.CancelEdit();
        }

        [TestMethod]
        public void EndEdit_TakesMessageOutOfEditMode()
        {            
            var eventWasRaised = false;            

            EditableMessage.BeginEdit();

            _message.PropertyChanged += (s, e) => eventWasRaised = e.PropertyName == "IsInEditMode";

            EditableMessage.EndEdit();

            Assert.IsTrue(eventWasRaised);
            Assert.IsFalse(_message.IsInEditMode);
        }

        [TestMethod]
        public void EndEdit_CanBeCalledOutsideEditScope_WithoutAnySideEffects()
        {
            EditableMessage.EndEdit();

            Assert.IsFalse(_message.IsInEditMode);
        }

        [TestMethod]
        public void EndEdit_CanBeCalledMultipleTimes_WithoutAnySideEffects()
        {
            EditableMessage.BeginEdit();
            EditableMessage.EndEdit();
            EditableMessage.EndEdit();
        }

        #endregion

        #region [====== CreateEditScope ======]

        [TestMethod]
        public void EditScope_BringsMessageIntoEditMode()
        {
            using (_message.CreateEditScope())
            {
                Assert.IsTrue(_message.IsInEditMode);                
            }
            Assert.IsFalse(_message.IsInEditMode);
        }

        [TestMethod]
        public void EditScope_RollsBackAllChanges_WhenCompleteIsNotCalled()
        {            
            using (_message.CreateEditScope())
            {
                _message.IntValue = 8;
                _message.Child = new TestMessage();              
            }
            Assert.AreEqual(0, _message.IntValue);
            Assert.IsNull(_message.Child);
        }

        [TestMethod]
        public void EditScope_CommitsAllChanges_WhenCompleteIsCalled()
        {
            var child = new TestMessage();

            using (var editScope = _message.CreateEditScope())
            {
                _message.IntValue = 8;
                _message.Child = child;

                editScope.Complete();
            }
            Assert.AreEqual(8, _message.IntValue);
            Assert.AreSame(child, _message.Child);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EditScope_Throws_IfCompleteIsCalledMoreThanOnce()
        {
            using (var editScope = _message.CreateEditScope())
            {
                editScope.Complete();
                editScope.Complete();
            }
        }

        #endregion
    }
}
