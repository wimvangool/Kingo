using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Messaging.Domain;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece
{
    [TestClass]
    public sealed class GameNotFoundScenario : InMemoryScenario<MovePieceCommand>
    {        
        protected override MessageToHandle<MovePieceCommand> When()
        {
            return new MovePieceCommand(Guid.NewGuid(), "e2", "e4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
