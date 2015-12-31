using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.RejectChallenge
{
    [TestClass]
    public sealed class ChallengeDoesNotExistScenario : MemoryScenario<RejectChallengeCommand>
    {        
        protected override RejectChallengeCommand When()
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
