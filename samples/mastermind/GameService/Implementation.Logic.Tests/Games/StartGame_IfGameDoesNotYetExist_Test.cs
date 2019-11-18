using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kingo.MicroServices;
using Kingo.MicroServices.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kingo.MasterMind.GameService.Games
{
    public sealed class StartGame_IfGameDoesNotYetExist_Test : MessageHandlerOperationTest<StartGameCommand, StartGame_IfGameDoesNotYetExist_Test.OutputStream>
    {
        #region [====== When ======]

        protected override Task WhenAsync(IMessageProcessor<StartGameCommand> processor, MicroProcessorOperationTestContext context)
        {
            return processor.ExecuteCommandAsync<StartGameHandler>(new StartGameCommand()
            {
                GameId = Guid.NewGuid(),
                PlayerName = "John"
            });
        }

        #endregion

        #region [====== Then ======]

        public sealed class OutputStream : MessageStream
        {
            internal OutputStream(MessageStream stream) :
                base(stream) { }

            public GameStartedEvent GetGameStartedEvent() =>
                GetMessage<GameStartedEvent>().Content;
        }

        protected override void Then(StartGameCommand message, IMessageHandlerOperationTestResult<OutputStream> result, MicroProcessorOperationTestContext context)
        {
            result.IsMessageStream(stream =>
            {
                var outputStream = new OutputStream(stream);
                AssertEvent(message, outputStream.GetGameStartedEvent());
                return outputStream;
            });
        }

        private static void AssertEvent(StartGameCommand message, GameStartedEvent @event)
        {
            Assert.AreEqual(message.GameId, @event.GameId);
            Assert.AreEqual(message.PlayerName, @event.PlayerName);
        }

        #endregion
    }
}
