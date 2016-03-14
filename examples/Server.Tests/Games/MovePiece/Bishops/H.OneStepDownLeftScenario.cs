using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Bishops
{
    [TestClass]
    public sealed class OneStepDownLeftScenario : MovePieceScenario
    {
        public OneStepDownLeftScenario()
        {
            TwoStepsDownRight = new TwoStepsDownRightScenario();
        }

        public TwoStepsDownRightScenario TwoStepsDownRight
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return TwoStepsDownRight.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return TwoStepsDownRight;
            yield return BlackPlayerMove("e3", "b6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("f4", "e3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
