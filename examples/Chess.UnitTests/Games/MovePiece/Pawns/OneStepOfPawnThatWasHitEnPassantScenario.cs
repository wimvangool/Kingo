using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class OneStepOfPawnThatWasHitEnPassantScenario : MovePieceScenario
    {
        public readonly EnPassantHitScenario EnPassantHit;

        public OneStepOfPawnThatWasHitEnPassantScenario()
        {
            EnPassantHit = new EnPassantHitScenario();
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return EnPassantHit;
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            // This move is illegal because the Pawn does not exist any more.
            return EnPassantHit.BlackPlayerMove("f5", "f4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
