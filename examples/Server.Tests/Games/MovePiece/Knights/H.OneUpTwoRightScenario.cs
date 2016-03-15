using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Knights
{
    [TestClass]
    public sealed class OneUpTwoRightScenario : MovePieceScenario
    {
        public OneUpTwoRightScenario()
        {
            TwoUpOneRight = new TwoUpOneRightScenario();
        }

        public TwoUpOneRightScenario TwoUpOneRight
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return TwoUpOneRight.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return TwoUpOneRight;
            yield return BlackPlayerMove("b8", "c6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("c3", "e4");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
