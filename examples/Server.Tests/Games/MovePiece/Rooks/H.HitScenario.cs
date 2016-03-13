using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Rooks
{
    [TestClass]
    public sealed class HitScenario : MovePieceScenario
    {
        public HitScenario()
        {
            SevenStepsRight = new SevenStepsRightScenario();
        }

        public SevenStepsRightScenario SevenStepsRight
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return SevenStepsRight.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return SevenStepsRight;
            yield return BlackPlayerMove("h6", "a6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("h3", "h5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
