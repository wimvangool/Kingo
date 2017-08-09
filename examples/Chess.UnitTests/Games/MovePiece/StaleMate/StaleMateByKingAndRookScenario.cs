using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.StaleMate
{
    [TestClass]
    public sealed class StaleMateByKingAndRookScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();

            #region [====== Knights removing most of the pieces ======]

            // 1.
            yield return WhitePlayerMove("e2", "e4");
            yield return BlackPlayerMove("e7", "e5");

            // 2.
            yield return WhitePlayerMove("g1", "f3");
            yield return BlackPlayerMove("g8", "f6");

            // 3.
            yield return WhitePlayerMove("d2", "d4");
            yield return BlackPlayerMove("b8", "c6");

            // 4.
            yield return WhitePlayerMove("d4", "e5");
            yield return BlackPlayerMove("c6", "e5");

            // 5.
            yield return WhitePlayerMove("f3", "e5");
            yield return BlackPlayerMove("f6", "e4");

            // 6.
            yield return WhitePlayerMove("e5", "f7");
            yield return BlackPlayerMove("e4", "f2");

            // 7.
            yield return WhitePlayerMove("f7", "h8");
            yield return BlackPlayerMove("f2", "h1");

            // 8.
            yield return WhitePlayerMove("h8", "g6");
            yield return BlackPlayerMove("h1", "g3");

            // 9.
            yield return WhitePlayerMove("g6", "f8");
            yield return BlackPlayerMove("g3", "f1");

            // 10.
            yield return WhitePlayerMove("f8", "e6");
            yield return BlackPlayerMove("f1", "e3");

            // 11.
            yield return WhitePlayerMove("e6", "d8");
            yield return BlackPlayerMove("e3", "d1");

            // 12.
            yield return WhitePlayerMove("d8", "c6");
            yield return BlackPlayerMove("d1", "c3");

            // 13.
            yield return WhitePlayerMove("c6", "a7");
            yield return BlackPlayerMove("c3", "a2");

            // 14.
            yield return WhitePlayerMove("a7", "c8");
            yield return BlackPlayerMove("a2", "c1");

            #endregion

            #region [====== Removing one Rook and two Knights ======]         

            // 15.
            yield return WhitePlayerMove("a1", "a8");
            yield return BlackPlayerMove("c1", "e2");

            // 16.
            yield return WhitePlayerMove("c8", "e7");
            yield return BlackPlayerMove("e8", "e7");

            // 17.
            yield return WhitePlayerMove("e1", "e2");
            yield return BlackPlayerMove("b7", "b5");

            #endregion

            #region [====== Removing most of the Pawns and Knight ======]

            // 18.
            yield return WhitePlayerMove("c2", "c4");
            yield return BlackPlayerMove("b5", "c4");

            // 19.
            yield return WhitePlayerMove("b2", "b3");
            yield return BlackPlayerMove("c4", "b3");

            // 20.
            yield return WhitePlayerMove("b1", "d2");
            yield return BlackPlayerMove("g7", "g5");

            // 21.
            yield return WhitePlayerMove("d2", "b3");
            yield return BlackPlayerMove("c7", "c5");

            // 22.
            yield return WhitePlayerMove("h2", "h4");
            yield return BlackPlayerMove("g5", "h4");

            // 23.
            yield return WhitePlayerMove("b3", "c5");
            yield return BlackPlayerMove("h4", "h3");

            // 24.
            yield return WhitePlayerMove("g2", "h3");
            yield return BlackPlayerMove("h7", "h5");

            // 25.
            yield return WhitePlayerMove("c5", "d7");
            yield return BlackPlayerMove("e7", "d7");

            #endregion

            #region [====== End Game ======]

            // 26.
            yield return WhitePlayerMove("h3", "h4");
            yield return BlackPlayerMove("d7", "c7");

            // 27.
            yield return WhitePlayerMove("a8", "a7");
            yield return BlackPlayerMove("c7", "c8");

            // 28.
            yield return WhitePlayerMove("e2", "e3");
            yield return BlackPlayerMove("c8", "d8");

            // 29.
            yield return WhitePlayerMove("e3", "e4");
            yield return BlackPlayerMove("d8", "e8");

            // 30.
            yield return WhitePlayerMove("e4", "e5");
            yield return BlackPlayerMove("e8", "f8");

            // 31.
            yield return WhitePlayerMove("e5", "f6");
            yield return BlackPlayerMove("f8", "g8");

            // 32.
            yield return WhitePlayerMove("f6", "g6");
            yield return BlackPlayerMove("g8", "h8");

            #endregion
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a7", "g7");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent(GameState.StaleMate);
        }
    }
}
