using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Bishops
{
    [TestClass]
    public sealed class TwoStepsDownRightScenario : MovePieceScenario
    {
        public TwoStepsDownRightScenario()
        {
            FourStepsUpRight = new FourStepsUpRightScenario();
        }

        public FourStepsUpRightScenario FourStepsUpRight
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return FourStepsUpRight.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return FourStepsUpRight;
            yield return BlackPlayerMove("h6", "e3");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("d6", "f4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
