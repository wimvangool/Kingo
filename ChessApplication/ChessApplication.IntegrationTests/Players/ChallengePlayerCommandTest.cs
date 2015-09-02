using System;
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
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.Errors.Count);
            Assert.IsNotNull(errorTree.Errors["SenderId"]);
        }

        #endregion

        #region [====== Validate - ReceiverId ======]

        [TestMethod]
        public void Validate_ReturnsErrors_IfReceiverIdIsEmpty()
        {
            var message = new ChallengePlayerCommand(Guid.NewGuid(), Guid.Empty);
            var errorTree = message.Validate();

            Assert.AreEqual(1, errorTree.Errors.Count);
            Assert.IsNotNull(errorTree.Errors["ReceiverId"]);
        }

        #endregion
    }
}