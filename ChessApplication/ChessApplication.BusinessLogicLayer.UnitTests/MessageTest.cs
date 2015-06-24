using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SummerBreeze.ChessApplication
{
    /// <summary>
    /// Serves as a base class for testing the behavior of a certain message.
    /// </summary>
    [TestClass]
    public abstract class MessageTest<TMessage>
        where TMessage : class, IMessage<TMessage>
    {
        #region [====== Constructors & Copying ======]        

        [TestMethod]
        public void Copy_ReturnsIdenticalCopyOfMessage()
        {
            var messageA = CreateValidMessage();
            var messageB = messageA.Copy();

            Assert.AreNotSame(messageA, messageB);
            Assert.AreEqual(messageA, messageB);
        }

        #endregion

        #region [====== Equals & GetHashCode ======]

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsComparedToNull()
        {
            var message = CreateValidMessage();

            Assert.AreNotEqual(message, null);
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageIsComparedToItself()
        {
            var message = CreateValidMessage();

            Assert.AreEqual(message, message);
        }

        [TestMethod]
        public void Equals_ReturnsTrue_IfMessageIsComparedToEqualInstance()
        {
            var messageA = CreateValidMessage();
            var messageB = CreateValidMessage();

            Assert.AreEqual(messageA, messageB);
        }

        [TestMethod]
        public void Equals_ReturnsFalse_IfMessageIsComparedToUnequalInstance()
        {
            var messageA = CreateValidMessage();
            var messageB = CreateUnequalCopyOf(messageA);

            Assert.AreNotEqual(messageA, messageB);
        }

        [TestMethod]
        public void GetHashCode_ReturnsSameValueForEqualInstances()
        {
            var messageA = CreateValidMessage();
            var messageB = messageA.Copy();

            Assert.AreEqual(messageA.GetHashCode(), messageB.GetHashCode());
        }

        #endregion

        #region [====== Validation - Valid Messages ======]

        [TestMethod]
        public void Validate_ReturnsNoErrors_IfMessageIsValid()
        {
            var message = CreateValidMessage();
            var errorTree = message.Validate();

            Assert.AreEqual(0, errorTree.TotalErrorCount);
        }

        #endregion

        #region [====== Message Factory Methods ======]

        protected abstract TMessage CreateValidMessage();

        protected abstract TMessage CreateUnequalCopyOf(TMessage message);

        #endregion
    }
}