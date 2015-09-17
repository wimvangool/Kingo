using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.ChessApplication.Players
{
    /// <summary>
    /// Tests the behavior of <see cref="ChallengePlayerCommand" /> instances.
    /// </summary>
    [TestClass]
    public sealed class ChallengePlayerCommandTest : MessageTest<ChallengePlayerCommand>
    {
        #region [====== Message Factory Methods ======]

        protected override ChallengePlayerCommand CreateValidMessage()
        {
            return new ChallengePlayerCommand(Guid.NewGuid(), Guid.NewGuid());
        }

        protected override ChallengePlayerCommand Change(ChallengePlayerCommand message)
        {
            return CreateValidMessage();
        }

        #endregion

        #region [====== Validate - SenderId ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfSenderIdIsEmpty()
        {
            var message = new ChallengePlayerCommand(Guid.Empty, Guid.NewGuid());
            var errorInfo = message.Validate().Single();

            Assert.AreEqual(1, errorInfo.Errors.Count);
            Assert.IsNotNull(errorInfo.Errors["SenderId"]);
        }

        #endregion

        #region [====== Validate - ReceiverId ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfReceiverIdIsEmpty()
        {
            var message = new ChallengePlayerCommand(Guid.NewGuid(), Guid.Empty);
            var errorInfo = message.Validate().Single();

            Assert.AreEqual(1, errorInfo.Errors.Count);
            Assert.IsNotNull(errorInfo.Errors["ReceiverId"]);
        }

        #endregion
    }
}