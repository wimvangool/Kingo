using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kingo.MicroServices;

namespace Kingo.MasterMind.GameService.Games
{
    public sealed class StartGameHandler : IMessageHandler<StartGameCommand>
    {
        public Task HandleAsync(StartGameCommand message, IMessageHandlerOperationContext context)
        {
            context.MessageBus.PublishEvent(new GameStartedEvent()
            {
                GameId = message.GameId,
                PlayerName = message.PlayerName
            });
            return Task.CompletedTask;
        }
    }
}
