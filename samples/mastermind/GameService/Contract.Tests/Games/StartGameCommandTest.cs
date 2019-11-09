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
            }).IsNotValid(1);
        }

        protected override StartGameCommand CreateRequest()
        {
            return new StartGameCommand()
            {
                GameId = Guid.NewGuid(),
                PlayerName = "Wim"
            };
        }
    }
}
