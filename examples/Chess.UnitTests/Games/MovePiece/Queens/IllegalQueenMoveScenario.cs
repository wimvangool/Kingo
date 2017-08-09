using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Queens
{

    [TestClass]
    public sealed class IllegalQueenMoveScenario : MovePieceScenario
    {
        public IllegalQueenMoveScenario()
        {
            FourStepsUpLeft = new FourStepsUpLeftScenario();
        }

        public FourStepsUpLeftScenario FourStepsUpLeft
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return FourStepsUpLeft.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return FourStepsUpLeft;
            yield return BlackPlayerMove("b7", "b5");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a4", "c5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
