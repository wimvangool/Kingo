using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceComponents.ComponentModel.Client
{
    [TestClass]
    public sealed class IsValidIndicatorTest
    {
        [TestMethod]
        public void IsValid_ReturnsTrue_WhenNoChildrenAreAdded()
        {
            var indicator = new IsValidIndicator();

            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValid_ReturnsTrue_WhenAddedChildIsValid()
        {
            var indicator = new IsValidIndicator()
            {
                new IsValidIndicatorStub(true)
            };

            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValid_ReturnsFalse_WhenAddedChildIsNotValid()
        {
            var indicator = new IsValidIndicator()
            {
                new IsValidIndicatorStub(false)
            };

            Assert.IsFalse(indicator.IsValid);
        }

        [TestMethod]
        public void IsValid_ReturnsFalse_WhenAnyAddedChildIsNotValid()
        {
            var indicator = new IsValidIndicator()
            {
                new IsValidIndicatorStub(false),
                new IsValidIndicatorStub(true)
            };

            Assert.IsFalse(indicator.IsValid);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddRange_Throws_IfCollectionIsNull()
        {
            var indicator = new IsValidIndicator();

            indicator.AddRange(null as IEnumerable<INotifyIsValid>);
        }

        [TestMethod]
        public void IsValidChanged_IsRaised_WhenChildIsAdded()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();

            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Add(new IsValidIndicatorStub(false));

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsRaised_WhenMultipleChildsAreAdded()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();

            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.AddRange(new IsValidIndicatorStub(true), new IsValidIndicatorStub(true));

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsNotRaised_WhenAddedChildIsNull()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();

            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Add(null);

            Assert.AreEqual(0, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsNotRaised_WhenAddedChildWasAlreadyAddedBefore()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();
            var child = new IsValidIndicatorStub(false);

            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Add(child);
            indicator.Add(child);

            Assert.AreEqual(2, raiseCount);
            Assert.IsFalse(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsRaised_WhenChildIsRemoved()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();
            var child = new IsValidIndicatorStub(false);

            indicator.Add(child);
            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Remove(child);

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsNotRaised_WhenChildToRemoveIsNull()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();
            var child = new IsValidIndicatorStub(false);

            indicator.Add(child);
            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Remove(null);

            Assert.AreEqual(0, raiseCount);
            Assert.IsFalse(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsNotRaised_WhenChildToRemoveDoesNotExist()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();
            var child = new IsValidIndicatorStub(false);

            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Remove(child);

            Assert.AreEqual(0, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsRaised_WhenIndicatorIsCleared()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();
            var child = new IsValidIndicatorStub(false);

            indicator.Add(child);
            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Clear();

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsNotRaised_WhenIndicatorIsClearedButWasAlreadyEmpty()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();

            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);
            indicator.Clear();

            Assert.AreEqual(0, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        [TestMethod]
        public void IsValidChanged_IsRaised_WhenChildIsValidChanged()
        {
            int raiseCount = 0;
            var indicator = new IsValidIndicator();
            var child = new IsValidIndicatorStub(false);

            indicator.Add(child);
            indicator.IsValidChanged += (s, e) => raiseCount++;
            indicator.PropertyChanged += (s, e) => IncrementIfIsValidChanged(e, ref raiseCount);

            child.IsValid = true;

            Assert.AreEqual(2, raiseCount);
            Assert.IsTrue(indicator.IsValid);
        }

        private static void IncrementIfIsValidChanged(PropertyChangedEventArgs e, ref int value)
        {
            if (e.PropertyName == "IsValid")
            {
                value++;
            }
        }
    }
}
