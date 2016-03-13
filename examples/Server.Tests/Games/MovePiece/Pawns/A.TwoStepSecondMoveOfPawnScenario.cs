using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class TwoStepSecondMoveOfPawnScenario : MovePieceScenario
    {
        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return base.Given().Concatenate();
            yield return WhitePlayerMove("e2", "e3");
            yield return BlackPlayerMove("d7", "d5");            
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e3", "e5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
