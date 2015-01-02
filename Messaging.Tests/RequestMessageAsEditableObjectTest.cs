using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel
{
    [TestClass]
    public sealed class RequestMessageAsEditableObjectTest
    {
        #region [====== TestMessage ======]

        private sealed class TestMessage : RequestMessage<TestMessage>
        {
            public TestMessage() { }

            private TestMessage(TestMessage message, bool makeReadOnly) : base(message, makeReadOnly)
            {
                _intValue = message._intValue;
                _child = AttachCopy(message._child);
            }

            public override TestMessage Copy(bool makeReadOnly)
            {
                return new TestMessage(this, makeReadOnly);
            }            

            private int _intValue;

            [RequiredConstraint]            
            public int IntValue
            {
                get { return _intValue; }
                set { SetValue(ref _intValue, value, () => IntValue); }
            }

            private TestMessage _child;
                        
            public TestMessage Child
            {
                get { return _child; }
                set { SetValue(ref _child, value, () => Child, PropertyChangedOption.MarkAsChanged); }
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

        #region [====== EditScope - Nesting ======]

        [TestMethod]
        public void EditScope_CanBeNested_IfNestingIsCorrect()
        {
            using (_message.CreateEditScope())
            {
                _message.IntValue = 3;

                using (_message.CreateEditScope())
                {
                    _message.IntValue = 5;
                }

                Assert.AreEqual(3, _message.IntValue);
            }
            Assert.AreEqual(0, _message.IntValue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EditScope_Throws_IfNestingOfCompleteIsIncorrect()
        {
            using (var outerScope = _message.CreateEditScope())
            {
                using (_message.CreateEditScope())
                {
                    outerScope.Complete();
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EditScope_Throws_IfNestingOfDisposeIsIncorrect()
        {
            using (var outerScope = _message.CreateEditScope())
            using (var innerScope = _message.CreateEditScope())
            {
                outerScope.Dispose();
                innerScope.Complete();
            }            
        }

        #endregion

        #region [====== EditScope - State ======]

        [TestMethod]
        public void State_IsNull_WhenPropertyIsChangedWithoutEditScope()
        {
            bool eventWasRaised = false;

            _message.PropertyChanged += (s, e) =>
            {
                var eventArgs = e as RequestMessagePropertyChangedEventArgs;
                if (eventArgs == null)
                {
                    return;
                }
                eventWasRaised = true;

                Assert.IsNull(eventArgs.State);
            };
            _message.IntValue = 3;

            Assert.IsTrue(eventWasRaised);
        }

        [TestMethod]
        public void ScopeId_IsPublishedInPropertyChangedEventArgs_WhenPropertyIsChangedWithinThatScope()
	    {
		    var nullStateCount = 0;
	
		    var outerScopeState = new object();
		    var outerScopeStateCount = 0;
		
		    var innerScopeState = new object();			
		    var innerScopeStateCount = 0;

            _message.PropertyChanged += (s, e) =>
            {
                var eventArgs = e as RequestMessagePropertyChangedEventArgs;
                if (eventArgs == null)
                {
                    return;
                }
                if (eventArgs.State == null)
                {
                    nullStateCount++;
                }
                else if (eventArgs.State == outerScopeState)
                {
                    outerScopeStateCount++;
                }
                else if (eventArgs.State == innerScopeState)
                {
                    innerScopeStateCount++;
                }
            };
		
		    _message.IntValue = 1;
		
		    using (var outerScope = _message.CreateEditScope(outerScopeState))
		    {
			    _message.IntValue = 2;
		
			    using (var innerScope = _message.CreateEditScope(innerScopeState))
			    {
				    _message.IntValue = 3;
			
				    innerScope.Complete();
			    }

                Assert.AreEqual(3, _message.IntValue);
			
			    _message.IntValue = 4;
			
			    outerScope.Complete();
            };

            Assert.AreEqual(4, _message.IntValue);

		    _message.IntValue = 5;

            Assert.AreEqual(5, _message.IntValue);
		
		    Assert.AreEqual(2, nullStateCount);
		    Assert.AreEqual(2, outerScopeStateCount);
		    Assert.AreEqual(1, innerScopeStateCount);
	    }

        #endregion

        #region [====== EditScope - SuppressValidation ======]

        [TestMethod]
        public void EditScope_SuppressesValidationInScope_WhenSuppressValidationIsTrue()
        {
            Assert.IsFalse(_message.IsValid);

            using (var editScope = _message.CreateEditScope(true))
            {
                // Normally, this would trigger validation.
                _message.IntValue = 4;

                Assert.IsFalse(_message.IsValid);

                // Validation will now be performed.
                editScope.Complete();
            }

            Assert.IsTrue(_message.IsValid);
        }

        [TestMethod]
        public void EditScope_WillStillSuppressValidation_IfOuterScopeSuppressesValidation()
        {
            Assert.IsFalse(_message.IsValid);

            using (var outerScope = _message.CreateEditScope(true))
            {
                // Normally, this would trigger validation.
                _message.IntValue = 4;

                Assert.IsFalse(_message.IsValid);

                using (var nestedScope = _message.CreateEditScope(false))
                {
                    // Normally, this would trigger validation, but the outer-scope prevents this.
                    _message.IntValue = 5;

                    Assert.IsFalse(_message.IsValid);

                    nestedScope.Complete();
                }

                Assert.IsFalse(_message.IsValid);

                // Validation will now be performed.
                outerScope.Complete();
            }

            Assert.IsTrue(_message.IsValid);
        }

        [TestMethod]
        public void EditScope_WillStillSuppressValidation_IfInnerScopeSuppressesValidation()
        {
            Assert.IsFalse(_message.IsValid);

            using (var outerScope = _message.CreateEditScope(false))
            {
                // This will trigger validation.
                _message.IntValue = 4;

                Assert.IsTrue(_message.IsValid);

                using (var nestedScope = _message.CreateEditScope(true))
                {
                    // Normally, this would trigger validation, but the inner-scope prevents this.
                    _message.IntValue = 0;

                    Assert.IsTrue(_message.IsValid);

                    // Validation will now be performed.
                    nestedScope.Complete();
                }

                Assert.IsFalse(_message.IsValid);
                
                outerScope.Complete();
            }

            Assert.IsFalse(_message.IsValid);
        }

        #endregion
    }
}
