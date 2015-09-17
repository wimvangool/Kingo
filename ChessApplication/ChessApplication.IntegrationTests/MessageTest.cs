using System.Linq;
using Kingo.BuildingBlocks.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication
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
        public void Equals_ReturnsFalse_IfMessageIsComparedToUnequalInstance()
        {
            var messageA = CreateValidMessage();
            var messageB = Change(messageA.Copy());

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
            var errorInfo = message.Validate().Single();

            Assert.AreEqual(0, errorInfo.Errors.Count);
        }

        #endregion

        #region [====== Message Factory Methods ======]

        protected abstract TMessage CreateValidMessage();

        protected abstract TMessage Change(TMessage message);

        #endregion
    }
}