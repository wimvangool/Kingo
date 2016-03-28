using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Pawns
{
    [TestClass]
    public sealed class OneStepFailedBecauseAwaitingPawnPromotion : MovePieceScenario
    {
        public OneStepFailedBecauseAwaitingPawnPromotion()
        {
            PawnMovedToEightRank = new PawnMovedToEightRankScenario();
        }

        public PawnMovedToEightRankScenario PawnMovedToEightRank
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return PawnMovedToEightRank.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return PawnMovedToEightRank;
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e2", "e4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
