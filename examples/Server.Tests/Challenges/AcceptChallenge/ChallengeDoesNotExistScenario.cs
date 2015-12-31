using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Challenges.AcceptChallenge
{
    [TestClass]
    public sealed class ChallengeDoesNotExistScenario : MemoryScenario<AcceptChallengeCommand>
    {        
        protected override AcceptChallengeCommand When()
        {
            return new AcceptChallengeCommand(Guid.NewGuid());
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
