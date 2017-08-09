using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Bishops
{
    [TestClass]
    public sealed class FourStepsUpRightScenario : MovePieceScenario
    {
        public FourStepsUpRightScenario()
        {
            ThreeStepsUpLeft = new ThreeStepsUpLeftScenario();
        }

        public ThreeStepsUpLeftScenario ThreeStepsUpLeft
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return ThreeStepsUpLeft.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return ThreeStepsUpLeft;
            yield return BlackPlayerMove("f8", "h6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a3", "d6");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
