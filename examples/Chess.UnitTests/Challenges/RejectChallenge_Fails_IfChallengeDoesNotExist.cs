using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class RejectChallenge_Fails_IfChallengeDoesNotExist : UnitTest<RejectChallengeCommand>
    {        
        protected override MessageToHandle<RejectChallengeCommand> When()
        {
            return new RejectChallengeCommand(Guid.NewGuid());
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
