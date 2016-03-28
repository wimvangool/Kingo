using System.Collections.Generic;
using System.Threading.Tasks;
using Kingo.Messaging;
using Kingo.Samples.Chess.Games.ChallengeAccepted;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.MovePiece.Rooks
{

    [TestClass]
    public sealed class SevenStepsRightScenario : MovePieceScenario
    {        
        public SevenStepsRightScenario()
        {
            TwoStepsForward = new TwoStepsUpScenario();
        }

        public TwoStepsUpScenario TwoStepsForward
        {
            get;
        }

        public override GameIsStartedScenario GameIsStarted
        {
            get { return TwoStepsForward.GameIsStarted; }
        }

        protected override IEnumerable<IMessageSequence> Given()
        {
            yield return TwoStepsForward;
            yield return BlackPlayerMove("h8", "h6");
        }

        protected override MessageToHandle<MovePieceCommand> When()
        {
            return WhitePlayerMove("a3", "h3");
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await ExpectPieceMovedEvent();
        }
    }
}
