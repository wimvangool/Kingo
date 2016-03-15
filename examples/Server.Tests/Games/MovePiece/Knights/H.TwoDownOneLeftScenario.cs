using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Knights
{
    [TestClass]
    public sealed class TwoDownOneLeftScenario : MovePieceScenario
    {
        public TwoDownOneLeftScenario()
        {
            OneUpTwoLeft = new OneUpTwoLeftScenario();
        }

        public OneUpTwoLeftScenario OneUpTwoLeft
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return OneUpTwoLeft.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return OneUpTwoLeft;
            yield return BlackPlayerMove("e5", "c4");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("c5", "b3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
