using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YellowFlare.MessageProcessing.Messages.Clients
{
    [TestClass]
    public sealed class CompositeIsBusyIndicatorTest
    {        
        [TestMethod]
        public void IsBusy_ReturnsFalse_WhenNoChildrenAreAdded()
        {
            var indicator = new CompositeIsBusyIndicator();

            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusy_ReturnsFalse_WhenAddedChildHasNoChanges()
        {
            var indicator = new CompositeIsBusyIndicator(new IsBusyIndicatorStub(false));

            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusy_ReturnsTrue_WhenAddedChildIsBusy()
        {
            var indicator = new CompositeIsBusyIndicator(new IsBusyIndicatorStub(true));

            Assert.IsTrue(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusy_ReturnsTrue_WhenAnyAddedChildHasNoChanges()
        {
            var indicator = new CompositeIsBusyIndicator(new IsBusyIndicatorStub(false), new IsBusyIndicatorStub(true));

            Assert.IsTrue(indicator.IsBusy);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRange_Throws_IfCollectionIsNull()
        {
            var indicator = new CompositeIsBusyIndicator();

            indicator.AddRange(null as IEnumerable<IIsBusyIndicator>);
        }

        [TestMethod]
        public void IsBusyChanged_IsRaised_WhenChildIsAdded()
        {
            int raiseCount = 0;
            var indicator = new CompositeIsBusyIndicator();

            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.Add(new IsBusyIndicatorStub(false));

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsRaised_WhenMultipleChildsAreAdded()
        {
            int raiseCount = 0;
            var indicator = new CompositeIsBusyIndicator();

            indicator.IsBusyChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsBusyChanged(e, ref raiseCount);
            indicator.AddRange(new IsBusyIndicatorStub(false), new IsBusyIndicatorStub(true));

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.IsBusy);
        }

        [TestMethod]
        public void IsBusyChanged_IsNotRaised_WhenAddedChildIsNull()
        {
            int raiseCount = 0;
            var indicator = new CompositeIsBusyIndicator();

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
            var indicator = new CompositeIsBusyIndicator();
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
            var indicator = new CompositeIsBusyIndicator();
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
            var indicator = new CompositeIsBusyIndicator();
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
            var indicator = new CompositeIsBusyIndicator();
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
            var indicator = new CompositeIsBusyIndicator();
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
            var indicator = new CompositeIsBusyIndicator();            
            
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
            var indicator = new CompositeIsBusyIndicator();
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
