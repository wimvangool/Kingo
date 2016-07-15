﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Check
{
    [TestClass]
    public sealed class SelfCheckByQueenScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("d1", "h5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return BlackPlayerMove("f7", "f6");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}