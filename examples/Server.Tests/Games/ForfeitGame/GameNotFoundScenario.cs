using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.Samples.Chess.Games.ForfeitGame
{
    [TestClass]
    public sealed class GameNotFoundScenario : InMemoryScenario<ForfeitGameCommand>
    {
        protected override ForfeitGameCommand When()
        {
            return new ForfeitGameCommand(Guid.NewGuid());
        }

        [TestMethod]
        public override async Task ThenAsync()
        {
            await Exception().Expect<CommandExecutionException>().ExecuteAsync();
        }
    }
}
