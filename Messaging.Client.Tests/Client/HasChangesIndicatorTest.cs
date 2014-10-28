using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Messaging.Client
{
    [TestClass]
    public sealed class HasChangesIndicatorTest
    {        
        [TestMethod]
        public void HasChanges_ReturnsFalse_WhenNoChildrenAreAdded()
        {
            var indicator = new HasChangesIndicator();

            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChanges_ReturnsFalse_WhenAddedChildHasNoChanges()
        {
            var indicator = new HasChangesIndicator()
            {
                new HasChangesIndicatorStub(false)
            };

            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChanges_ReturnsTrue_WhenAddedChildHasChanges()
        {
            var indicator = new HasChangesIndicator()
            {
                new HasChangesIndicatorStub(true)
            };

            Assert.IsTrue(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChanges_ReturnsTrue_WhenAnyAddedChildHasNoChanges()
        {
            var indicator = new HasChangesIndicator()
            {
                new HasChangesIndicatorStub(false),
                new HasChangesIndicatorStub(true)
            };

            Assert.IsTrue(indicator.HasChanges);
        }       

        [TestMethod]
        public void HasChangesChanged_IsRaised_WhenChildIsAdded()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();

            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Add(new HasChangesIndicatorStub(false));

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }        

        [TestMethod]
        public void HasChangesChanged_IsNotRaised_WhenAddedChildIsNull()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();

            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Add(null);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChangesChanged_IsNotRaised_WhenAddedChildWasAlreadyAddedBefore()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();
            var child = new HasChangesIndicatorStub(false);

            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Add(child);
            indicator.Add(child);

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChangesChanged_IsRaised_WhenChildIsRemoved()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();
            var child = new HasChangesIndicatorStub(false);
            
            indicator.Add(child);
            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Remove(child);

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }        

        [TestMethod]
        public void HasChangesChanged_IsNotRaised_WhenChildToRemoveIsNull()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();
            var child = new HasChangesIndicatorStub(false);

            indicator.Add(child);
            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Remove(null);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChangesChanged_IsNotRaised_WhenChildToRemoveDoesNotExist()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();
            var child = new HasChangesIndicatorStub(false);
            
            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Remove(child);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChangesChanged_IsRaised_WhenIndicatorIsCleared()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();
            var child = new HasChangesIndicatorStub(false);

            indicator.Add(child);
            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Clear();

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChangesChanged_IsNotRaised_WhenIndicatorIsClearedButWasAlreadyEmpty()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();            
            
            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);
            indicator.Clear();

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.HasChanges);
        }

        [TestMethod]
        public void HasChangesChanged_IsRaised_WhenChildHasChangesChanged()
        {
            int raiseCount = 0;
            var indicator = new HasChangesIndicator();
            var child = new HasChangesIndicatorStub(false);

            indicator.Add(child);
            indicator.HasChangesChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfHasChangesChanged(e, ref raiseCount);

            child.HasChanges = true;

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.HasChanges);
        }

        private static void IncrementIfHasChangesChanged(PropertyChangedEventArgs e, ref int value)
        {
            if (e.PropertyName == "HasChanges")
            {
                value++;
            }
        }
    }
}
