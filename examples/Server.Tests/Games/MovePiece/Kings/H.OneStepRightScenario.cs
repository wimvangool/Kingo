using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Kings
{
    [TestClass]
    public sealed class OneStepRightScenario : MovePieceScenario
    {
        public OneStepRightScenario()
        {
            OneStepUp = new OneStepUpScenario();
        }

        public OneStepUpScenario OneStepUp
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return OneStepUp.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return OneStepUp;
            yield return BlackPlayerMove("e8", "e7");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e2", "f2");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
