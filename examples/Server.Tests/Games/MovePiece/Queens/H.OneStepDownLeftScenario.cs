using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Queens
{
    [TestClass]
    public sealed class OneStepDownLeftScenario : MovePieceScenario
    {
        public OneStepDownLeftScenario()
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
            yield return BlackPlayerMove("a5", "h5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("h4", "g3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
