﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Kings
{
    [TestClass]
    public sealed class CastlingOfWhiteFailedBecauseKingMovedScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");
            yield return WhitePlayerMove("d2", "d3");
            yield return BlackPlayerMove("g8", "f6");
            yield return WhitePlayerMove("g1", "f3");
            yield return BlackPlayerMove("g7", "g6");
            yield return WhitePlayerMove("g2", "g3");
            yield return BlackPlayerMove("b8", "c6");
            yield return WhitePlayerMove("f1", "g2");
            yield return BlackPlayerMove("f8", "b4");
            yield return WhitePlayerMove("e1", "e2");
            yield return BlackPlayerMove("b4", "c5");
            yield return WhitePlayerMove("e2", "e1");
            yield return BlackPlayerMove("b7", "b6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e1", "g1");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}