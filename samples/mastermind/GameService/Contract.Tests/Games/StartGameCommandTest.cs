using System;
using System.Collections.Generic;
using System.Text;
using Kingo.MicroServices.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MasterMind.GameService.Games
{
    [TestClass]
    public sealed class StartGameCommandTest : RequestMessageTest<StartGameCommand>
    {
        #region [====== IsNotValid (GameId) ======]

        [TestMethod]
        public void Command_IsNotValid_IfGameIdIsNotSpecified()
        {
            AssertThat(command =>
            {
                command.GameId = Guid.Empty;

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.GameId)].HasError("GameId must have a non-default value.");
            });
        }

        #endregion

        #region [====== IsNotValid (PlayerName) ======]

        [TestMethod]
        public void Command_IsNotValid_IfPlayerNameIsNotSpecified()
        {
            AssertThat(command =>
            {
                command.PlayerName = null;

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.PlayerName)].HasError("The PlayerName field is required.");
            });
        }

        [TestMethod]
        public void Command_IsNotValid_IfPlayerNameIsEmpty()
        {
            AssertThat(command =>
            {
                command.PlayerName = string.Empty;

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.PlayerName)].HasError("The PlayerName field is required.");
            });
        }

        [TestMethod]
        public void Command_IsNotValid_IfPlayerNameContainsOnlyWhiteSpace()
        {
            AssertThat(command =>
            {
                command.PlayerName = " ";

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.PlayerName)].HasError("The PlayerName field is required.");
            });
        }

        [TestMethod]
        public void Command_IsNotValid_IfPlayerNameContainsInvalidCharacters()
        {
            AssertThat(command =>
            {
                command.PlayerName = "Abcd3";

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.PlayerName)].HasError("The PlayerName may only contain letters.");
            });
        }

        #endregion

        #region [====== IsValid ======]

        [TestMethod]
        public void Command_IsValid_IfAllValidationConstraintsAreSatisfied()
        {
            AssertThat(CreateValidRequestMessage()).IsValid();
        }

        protected override StartGameCommand CreateValidRequestMessage()
        {
            return new StartGameCommand()
            {
                GameId = Guid.NewGuid(),
                PlayerName = "John"
            };
        }

        #endregion
    }
}
