using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Knights
{
    [TestClass]
    public sealed class OneUpTwoLeftScenario : MovePieceScenario
    {
        public OneUpTwoLeftScenario()
        {
            OneUpTwoRight = new OneUpTwoRightScenario();
        }

        public OneUpTwoRightScenario OneUpTwoRight
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return OneUpTwoRight.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return OneUpTwoRight;
            yield return BlackPlayerMove("c6", "e5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("e4", "c5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
