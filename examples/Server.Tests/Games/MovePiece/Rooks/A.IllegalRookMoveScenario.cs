using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Rooks
{
    [TestClass]
    public sealed class IllegalRookMoveScenario : MovePieceScenario
    {
        public IllegalRookMoveScenario()
        {
            TwoStepsFoward = new TwoStepsFowardScenario();
        }

        public TwoStepsFowardScenario TwoStepsFoward
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return TwoStepsFoward.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return TwoStepsFoward;
            yield return BlackPlayerMove("a7", "a6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a3", "c5");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectedCommandExecutionException();
        }
    }
}
