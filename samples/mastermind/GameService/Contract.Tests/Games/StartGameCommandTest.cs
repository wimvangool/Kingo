using System;
using System.Collections.Generic;
using System.Text;
using Kingo.Games;
using Kingo.MicroServices.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MasterMind.GameService.Games
{
    [TestClass]
    public sealed class StartGameCommandTest : RequestMessageTest<StartGameCommand>
    {
        [TestMethod]
        public void Command_IsNotValid_IfGameIdIsNotSpecified()
        {
            AssertThat(command =>
            {
                command.GameId = Guid.Empty;

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.GameId)].HasError("'GameId' is not allowed to have its default value.");
            });
        }

        [TestMethod]
        public void Command_IsNotValid_IfPlayerNameIsNotSpecified()
        {
            AssertThat(command =>
            {
                command.PlayerName = null;

            }).IsNotValid(1).And(members =>
            {
                members[nameof(StartGameCommand.PlayerName)].HasError("'PlayerName' is required");
            });
        }

        protected override StartGameCommand CreateValidRequestMessage()
        {
            return new StartGameCommand()
            {
                GameId = Guid.NewGuid(),
                PlayerName = "John"
            };
        }
    }
}
