using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Client
{
    [TestClass]
    public sealed class IsBusyIndicatorTest
    {        
        [TestMethod]
        public void IsBusy_ReturnsFalse_WhenNoChildrenAreAdded()
        {
            var indicator = new IsBusyIndicator();

            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusy_ReturnsFalse_WhenAddedChildHasNoChanges()
        {
            var indicator = new IsBusyIndicator()
            {
                new IsBusyIndicatorStub(false)
            };

            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusy_ReturnsTrue_WhenAddedChildIsBusy()
        {
            var indicator = new IsBusyIndicator()
            {
                new IsBusyIndicatorStub(true)
            };

            Assert.IsTrue(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusy_ReturnsTrue_WhenAnyAddedChildHasNoChanges()
        {
            var indicator = new IsBusyIndicator()
            {
                new IsBusyIndicatorStub(false),
                new IsBusyIndicatorStub(true)
            };

            Assert.IsTrue(indicator.IsBusy);
        }        

        [TestMethod]
        public void IsBusyChanged_IsRaised_WhenChildIsAdded()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();

            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Add(new IsBusyIndicatorStub(false));

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }        

        [TestMethod]
        public void IsBusyChanged_IsNotRaised_WhenAddedChildIsNull()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();

            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Add(null);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsNotRaised_WhenAddedChildWasAlreadyAddedBefore()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();
            var child = new IsBusyIndicatorStub(false);

            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Add(child);
            indicator.Add(child);

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsRaised_WhenChildIsRemoved()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();
            var child = new IsBusyIndicatorStub(false);
            
            indicator.Add(child);
            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Remove(child);

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }        

        [TestMethod]
        public void IsBusyChanged_IsNotRaised_WhenChildToRemoveIsNull()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();
            var child = new IsBusyIndicatorStub(false);

            indicator.Add(child);
            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Remove(null);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsNotRaised_WhenChildToRemoveDoesNotExist()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();
            var child = new IsBusyIndicatorStub(false);
            
            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Remove(child);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsRaised_WhenIndicatorIsCleared()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();
            var child = new IsBusyIndicatorStub(false);

            indicator.Add(child);
            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Clear();

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsNotRaised_WhenIndicatorIsClearedButWasAlreadyEmpty()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();            
            
            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Clear();

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsRaised_WhenChildIsBusyChanged()
        {
            int raiseCount = 0;
            var indicator = new IsBusyIndicator();
            var child = new IsBusyIndicatorStub(false);

            indicator.Add(child);
            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);

            child.IsBusy = true;

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.IsBusy);
        }

        private static void IncrementIfIsBusyChanged(PropertyChangedEventArgs e, ref int value)
        {
            if (e.PropertyName == "IsBusy")
            {
                value++;
            }
        }
    }
}
