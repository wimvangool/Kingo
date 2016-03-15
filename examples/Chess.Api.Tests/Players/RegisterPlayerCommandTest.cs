using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Players
{
    [TestClass]
    public sealed class RegisterPlayerCommandTest
    {
        #region [====== PlayerId ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfPlayerIdIsEmpty()
        {
            var message = new RegisterPlayerCommand(Guid.Empty, NewPlayerName());

            message.Validate().AssertMemberError("'PlayerId' is not specified.", "PlayerId");
        }        

        private static Guid NewPlayerId()
        {
            return Guid.NewGuid();
        }

        #endregion

        #region [====== PlayerName ======]

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfPlayerNameIsNull()
        {
            var message = new RegisterPlayerCommand(NewPlayerId(), null);

            message.Validate().AssertMemberError("'PlayerName' is not specified.", "PlayerName");
        }

        [TestMethod]
        public void Validate_ReturnsExpectedError_IfPlayerNameIsNoIdentifier()
        {
            var message = new RegisterPlayerCommand(NewPlayerId(), "%$");

            message.Validate().AssertMemberError("PlayerName (%$) is not a valid name.", "PlayerName");
        }        

        private static string NewPlayerName()
        {
            return "John";
        }

        #endregion

        #region [====== Valid Messages ======]

        [TestMethod]
        public void Validate_ReturnsNoError_IfMessageIsValid()
        {
            var message = new RegisterPlayerCommand(NewPlayerId(), NewPlayerName());

            message.Validate().AssertNoErrors();
        }

        #endregion
    }
}
