using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Knights
{
    [TestClass]
    public sealed class TwoUpOneLeftScenario : MovePieceScenario
    {
        public TwoUpOneLeftScenario()
        {
            TwoDownOneLeft = new TwoDownOneLeftScenario();
        }

        public TwoDownOneLeftScenario TwoDownOneLeft
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return TwoDownOneLeft.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return TwoDownOneLeft;
            yield return BlackPlayerMove("c4", "b6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("b3", "a5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
