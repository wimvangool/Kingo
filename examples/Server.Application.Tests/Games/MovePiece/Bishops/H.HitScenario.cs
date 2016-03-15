using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Bishops
{
    [TestClass]
    public sealed class HitScenario : MovePieceScenario
    {
        public HitScenario()
        {
            OneStepDownLeft = new OneStepDownLeftScenario();
        }

        public OneStepDownLeftScenario OneStepDownLeft
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return OneStepDownLeft.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return OneStepDownLeft;
            yield return BlackPlayerMove("h7", "h5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e3", "b6");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
