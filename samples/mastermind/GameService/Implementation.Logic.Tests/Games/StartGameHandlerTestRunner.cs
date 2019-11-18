using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MasterMind.GameService.Games
{
    [TestClass]
    public sealed class StartGameHandlerTestRunner : BusinessLogicTestRunner
    {
        [TestMethod]
        public Task MessageHandler_ReturnsExpectedStream_IfGameDoesNotYetExist() =>
            RunAsync(new StartGame_IfGameDoesNotYetExist_Test());
    }
}
