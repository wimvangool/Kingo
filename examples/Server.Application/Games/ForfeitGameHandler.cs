using System;
using System.Threading.Tasks;
using Kingo.Messaging;
using PostSharp.Patterns.Contracts;

namespace Kingo.Samples.Chess.Games
{
    public sealed class ForfeitGameHandler : IMessageHandler<ForfeitGameCommand>
    {
        private readonly IGameRepository _games;

        public ForfeitGameHandler([NotNull] IGameRepository games)
        {            
            _games = games;
        }

        public async Task HandleAsync([NotNull] ForfeitGameCommand message)
        {            
            var game = await _games.GetByKeyAsync(message.GameId);

            game.Forfeit(Session.Current.PlayerId);
        }
    }
}
